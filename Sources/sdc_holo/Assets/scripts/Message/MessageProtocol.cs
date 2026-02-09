// 放在 Assets/Scripts/Network/Protocol/MessageProtocol.cs

public enum MessageCommand
{
    SPAWN,
    UPDATE,
    DELETE
}

public enum TargetType
{
    DEBRIS,
    CATCHER
}