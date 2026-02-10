<#-- Auto generated file -->
ent-LogicEmptyCircuit = пустая плата
    .desc = Кажется, чего-то не хватает.

ent-LogicGateOr = логический элемент
    .desc = Логический элемент с двумя портами на вход и одним на выход. Можно изменить логическую операцию с помощью отвёртки.
    .suffix = Or, ИЛИ

ent-EdgeDetector = детектор сигнала
    .desc = Определяет уровень сигнала и разделяет его. Устройство игнорирует импульсные сигналы.

ent-PowerSensor = датчик питания
    .desc = Генерирует сигналы в ответ на изменение напряжения в сети. Может циклически переключаться между напряжениями кабеля.

ent-MemoryCell = ячейка памяти
    .desc = Схема D-триггер защёлки, хранящая сигнал, который может быть изменён в зависимости от входного и разрешающего портов.

ent-RandomGate = ent-RandomGate = случайный логический элемент
    .desc = Логический элемент, который выдает случайный сигнал при изменении входного сигнала.

ent-LogicGateAnd = { ent-LogicGateOr }
    .suffix = And, И
    .desc = { ent-LogicGateOr }

ent-LogicGateXor = { ent-LogicGateOr }
    .suffix = Xor, Исключающее ИЛИ
    .desc = { ent-LogicGateOr }

ent-LogicGateNor = { ent-LogicGateOr }
    .suffix = Nor, ИЛИ-НЕ
    .desc = { ent-LogicGateOr }

ent-LogicGateNand = { ent-LogicGateOr }
    .suffix = Nand, И-НЕ
    .desc = { ent-LogicGateOr }

ent-LogicGateXnor = {ent-LogicGateOr}
    .suffix = Xnor, Исключающее ИЛИ-НЕ
    .desc = {ent-LogicGateOr}
