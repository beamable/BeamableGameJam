using System;
using System.Collections.Generic;
using Beamable;
using Beamable.Common.Content;

[Agnostic]
[Serializable]
public class CommandsConfigRef<TContent> : ContentRef<TContent> where TContent : CommandsConfig, new()
{
}

[Serializable]
[Agnostic]
public class CommandsConfigRef : CommandsConfigRef<CommandsConfig>
{
}

[ContentType("CommandsConfig")]
[Serializable]
[Agnostic]
public class CommandsConfig : Config
{
    public List<string> AvailableCommands = new List<string>();
}
