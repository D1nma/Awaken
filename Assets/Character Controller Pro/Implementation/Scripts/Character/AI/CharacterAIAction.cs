﻿using Lightbug.Utilities;

namespace Lightbug.CharacterControllerPro.Implementation
{

public enum SequenceType
{
    Duration ,
    OnWallHit
}

/// <summary>
/// This class represents a sequence action, executed by the AI brain in sequence behaviour mode.
/// </summary>
[System.Serializable]
public class CharacterAIAction
{
    public SequenceType sequenceType;

	[Range_NoSlider( true )]
	public float duration = 1;

	public CharacterActionsInfo action = new CharacterActionsInfo();
    
}

}