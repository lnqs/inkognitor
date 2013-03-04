# -*- coding: utf-8 -*- 
<%inherit file="layout.mako"/>

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
<div class="row">
    <div class="span6">
        <h4>Wartungsmodus startet automatisch</h4>
        <p>${'Startet automatisch' if maintainance_may_start else 'Startet nicht automatisch'}</p>
        <a class="btn" href="${request.route_url('execute', command='enable_maintainance_may_start')}">
            Aktivieren
        </a>
        <a class="btn" href="${request.route_url('execute', command='disable_maintainance_may_start')}">
            Deaktivieren
        </a>
    </div>
    <div class="span6">
        <h4>Bot antwortet</h4>
        <p>${'Der Bot antwortet' if bot_may_answer else 'Der Bot antwortet nicht'}</p>
		<a class="btn" href="${request.route_url('execute', command='enable_bot_may_answer')}">
            Aktivieren
        </a>
        <a class="btn" href="${request.route_url('execute', command='disable_bot_may_answer')}">
            Deaktivieren
        </a>
    </div>
</div>
