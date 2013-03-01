# -*- coding: utf-8 -*- 
<%inherit file="layout.mako"/>

<ul>
% for line in loglines:
    <li>${line}</li>
% endfor
</ul>
