#!/usr/bin/env python

import os
import socket
import select
from pyramid.config import Configurator
from pyramid.view import view_config
from pyramid.httpexceptions import HTTPFound, HTTPNotFound, HTTPBadRequest
from wsgiref.simple_server import make_server


LISTENING_ADDRESS = '0.0.0.0'
LISTENING_PORT = 8080
INKOGNITOR_ADDRESS = '127.0.0.1'
INKOGNITOR_PORT = 13135
CHATLOG = 'Chat.log'
SOCKET_TIMEOUT = 3


COMMANDS = {
    'prev_mode': ('prev_mode', 'Modus gewechselt'),
    'next_mode': ('next_mode', 'Modus gewechselt'),
    'say': ('say "{text}"', 'Text wird ausgegeben'),
    'enable_maintainance_may_start': ('set_maintainance_may_start True', 'Gespeichert'),
    'disable_maintainance_may_start': ('set_maintainance_may_start False', 'Gespeichert'),
    'enable_bot_may_answer': ('set_bot_may_answer True', 'Gespeichert'),
    'disable_bot_may_answer': ('set_bot_may_answer False', 'Gespeichert')
}


REQUESTS = {
    'mode': 'get_mode_name',
    'show_maintainance_may_start': 'show_maintainance_may_start',
    'show_bot_may_answer': 'show_bot_may_answer'
}


def communicate_inkognitor(data):
    sock = socket.create_connection((INKOGNITOR_ADDRESS, INKOGNITOR_PORT))
    try:
        sock.sendall(data + '\n')
        response = ''

        while True:
            ready = select.select([sock], [], [], SOCKET_TIMEOUT)
            if ready[0]:
                new_data = sock.recv(1024)
                if new_data == '':
                    break
                response += new_data
            else:
                raise socket.timeout('receiving data took too much time')

        return response
    finally:
        sock.close()


@view_config(route_name='main', renderer='main.mako')
@view_config(route_name='execute', renderer='main.mako')
def main_view(request):
    data = {
        'active_page': 'main',
        'current_mode': '',
        'maintainance_may_start': False,
        'bot_may_answer': True,
        'message': ''
    }

    if 'command' in request.matchdict:
        command = request.matchdict['command']
        if command in COMMANDS:
            userdata = dict(request.GET, **request.POST)
            commandstring = COMMANDS[command][0]
            try:
                response = communicate_inkognitor(commandstring.format(**userdata))
                data['message'] = COMMANDS[command][1].format(response)
            except KeyError:
                return HTTPBadRequest()
            except socket.error as e:
                data['message'] = 'Error: Communication with Inkognitor failed: {}'.format(e)
            except socket.timeout as e:
                data['message'] = 'Error: Communication with Inkognitor timed out: {}'.format(e)
        else:
            data['message'] = 'Error: Invalid Command'

    try:
        data['current_mode'] = communicate_inkognitor(REQUESTS['mode'])
        data['maintainance_may_start'] = communicate_inkognitor(
		    REQUESTS['show_maintainance_may_start']).strip() == 'True'
        data['bot_may_answer'] = communicate_inkognitor(
		    REQUESTS['show_bot_may_answer']).strip() == 'True'
    except socket.error as e:
        data['message'] = 'Error: Communication with Inkognitor failed: {}'.format(e)

    return data


@view_config(route_name='chatlog', renderer='chatlog.mako')
def chatlog_view(request):
    data = {
        'active_page': 'chatlog',
        'loglines': [],
        'message:': ''
    }
    
    try:
        with open(CHATLOG, 'r') as f:
            data['loglines'] = f.readlines()
    except Exception as e:
        data['message'] = 'Error: Couldn\'t read logfile: {}'.format(e)

    return data


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