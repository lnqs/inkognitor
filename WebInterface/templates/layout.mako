# -*- coding: utf-8 -*- 
<!DOCTYPE html>  
<html>
<head>
    <meta charset="utf-8">
    <title>Inkognitor</title>
    <link href="/static/css/bootstrap.min.css" rel="stylesheet" media="screen">
</head>

<body>

<div class="container">
    <div class="masthead">
        <ul class="nav nav-pills pull-right">
            <li class="${'active' if active_page == 'main' else ''}">
                <a href="${request.route_url('main')}">Konfiguration</a>
            </li>
            <li class="${'active' if active_page == 'chatlog' else ''}">
                <a href="${request.route_url('chatlog')}">Chatlog</a>
            </li>
        </ul>
        <h3 class="muted">Inkognitor Web-Interface</h3>
    </div>
    <hr />
    ${next.body()}
</div>
<script src="/static/js/bootstrap.min.js"></script>

</body>
</html>
