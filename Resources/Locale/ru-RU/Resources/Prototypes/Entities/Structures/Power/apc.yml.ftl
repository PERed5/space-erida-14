<#-- Auto generated file -->
ent-BaseAPC = ЛКП
    .desc = Терминал управления локальными электрическими системами.

ent-APCFrame = каркас ЛКП
    .desc = Терминал управления локальными электрическими системами, без электроники.

ent-APCConstructed = { ent-BaseAPC }
    .suffix = Открыт
    .desc = { ent-BaseAPC }

ent-APCBasic = { ent-BaseAPC }
    .suffix = Базовый, 50кДж
    .desc = { ent-BaseAPC }

ent-APCHighCapacity = { ent-BaseAPC }
    .suffix = Высокая ёмкость, 100кДж
    .desc = { ent-BaseAPC }

ent-APCSuperCapacity = { ent-BaseAPC }
    .suffix = Супер ёмкость, 150кДж
    .desc = { ent-BaseAPC }

ent-APCHyperCapacity = { ent-BaseAPC }
    .suffix = Гипер ёмкость, 200кДж
    .desc = { ent-BaseAPC }

ent-APCXenoborg = {ent-BaseAPC}
    .suffix = Базовый, 50кДж, Ксеноборг
    .desc = {ent-BaseAPC}
