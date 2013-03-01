#!/usr/bin/env python

import os
import socket
from pyramid.config import Configurator
from pyramid.view import view_config
from pyramid.httpexceptions import HTTPFound, HTTPNotFound, HTTPBadRequest
from wsgiref.simple_server import make_server


LISTENING_ADDRESS = '0.0.0.0'
LISTENING_PORT = 8080
INKOGNITOR_ADDRESS = '127.0.0.1'
INKOGNITOR_PORT = 13135
CHATLOG = 'Chat.log'


COMMANDS = {
    'prev_mode': ('prev_mode', 'Modus gewechselt'),
    'next_mode': ('next_mode', 'Modus gewechselt'),
    'say': ('say "{text}"', 'Text wird ausgegeben')
}


REQUESTS = {
    'mode': 'get_mode_name'
}


def communicate_inkognitor(data):
    sock = socket.create_connection((INKOGNITOR_ADDRESS, INKOGNITOR_PORT))
    try:
        sock.sendall(data)
        response = ''
        while True:
            new_data = sock.recv(1024)
            response += new_data
            if new_data == '':
                break
        return response
    finally:
        sock.close()


@view_config(route_name='main', renderer='main.mako')
@view_config(route_name='execute', renderer='main.mako')
def main_view(request):
    data = {
        'active_page': 'main',
        'current_mode': '',
        'message': ''
    }

    try:
        message = '{}\n'.format(REQUESTS['mode'])
        data['mode'] = communicate_inkognitor(message)
    except socket.error:
        data['message'] = 'Error: Communication with Inkognitor failed: {}'.format(e)

    if 'command' in request.matchdict:
        command = request.matchdict['command']
        if command in COMMANDS:
            userdata = dict(request.GET, **request.POST)
            commandstring = '{}\n'.format(COMMANDS[command][0])
            try:
                response = communicate_inkognitor(commandstring.format(**userdata))
                data['message'] = COMMANDS[command][1].format(response)
            except KeyError:
                return HTTPBadRequest()
            except socket.error as e:
                data['message'] = 'Error: Communication with Inkognitor failed: {}'.format(e)
        else:
            data['message'] = 'Error: Invalid Command'

    return data


@view_config(route_name='chatlog', renderer='chatlog.mako')
def chatlog_view(request):
    try:
        with open(CHATLOG, 'r') as f:
            loglines = f.readlines()
    except:
        loglines = ['Error: Couldn\'t read logfile']

    return {
        'active_page': 'chatlog',
        'loglines': loglines
    }


if __name__ == '__main__':
    here = os.path.dirname(os.path.abspath(__file__))

    settings = {
        'mako.directories': os.path.join(here, 'templates')
    }

    config = Configurator(settings=settings)
    config.add_route('main', '/')
    config.add_route('execute', '/execute/{command}')
    config.add_route('chatlog', '/chatlog')
    config.add_static_view('static', os.path.join(here, 'static'))
    config.scan()

    server = make_server(LISTENING_ADDRESS, LISTENING_PORT, config.make_wsgi_app())
    server.serve_forever()