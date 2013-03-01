# -*- coding: utf-8 -*- 
<%inherit file="layout.mako"/>

% if message != '':
<div class="hero-unit">
    ${message}
</div>
% endif

<div class="row">
    <div class="span6">
        <h4>Sprachausgabe</h4>
        <form action="${request.route_url('execute', command='say')}" method="post">
            <input type="text" name="text"><br />
            <input type="submit" value="Ausgeben" class="btn">
        </form>
    </div>
    <div class="span6">
        <h4>Modus</h4>
        <p>Aktiv: ${current_mode}</p>
        <a class="btn" href="${request.route_url('execute', command='prev_mode')}">
            &laquo; Vorheriger
        </a>
        <a class="btn" href="${request.route_url('execute', command='next_mode')}">
            N&auml;chster &raquo;
        </a>
    </div>
</div>
