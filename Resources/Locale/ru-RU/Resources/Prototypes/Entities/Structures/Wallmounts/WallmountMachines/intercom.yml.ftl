<#-- Auto generated file -->
ent-IntercomAssembly = каркас интеркома
    .desc = Интерком. На данный момент он бесполезен.

ent-IntercomConstructed = { ent-BaseIntercom }
    .suffix = Пустой, Панель открыта
    .desc = { ent-BaseIntercom }

ent-Intercom = { ent-IntercomConstructed }
    .desc = { ent-IntercomConstructed }

ent-IntercomCommon = { ent-Intercom }
    .suffix = Общий
    .desc = { ent-Intercom }

ent-IntercomCommand = { ent-BaseIntercomSecure }
    .desc = Интерком. Он был укреплён металлом.
    .suffix = Командный

ent-IntercomEngineering = { ent-Intercom }
    .suffix = Инженерный
    .desc = { ent-Intercom }

ent-IntercomMedical = { ent-Intercom }
    .suffix = Медицинский
    .desc = { ent-Intercom }

ent-IntercomScience = { ent-Intercom }
    .suffix = Научный
    .desc = { ent-Intercom }

ent-IntercomSecurity = { ent-BaseIntercomSecure }
    .desc = Интерком. Он был укреплён металлом из шлемов охраны, поэтому открыть его не так-то просто.
    .suffix = Служба безопасности

ent-IntercomService = { ent-Intercom }
    .suffix = Сервисный
    .desc = { ent-Intercom }

ent-IntercomSupply = { ent-Intercom }
    .suffix = Снабжение
    .desc = { ent-Intercom }

ent-IntercomAll = { ent-Intercom }
    .suffix = Все
    .desc = { ent-Intercom }

ent-IntercomFreelance = {ent-Intercom}
    .suffix = Фриланс
    .desc = {ent-Intercom}
