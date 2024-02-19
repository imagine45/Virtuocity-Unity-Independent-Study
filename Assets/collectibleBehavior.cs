using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectibleBehavior : MonoBehaviour
{
    private enum respawnStates { RESPAWNS, DOES_NOT_RESPAWN = 1 }
    [SerializeField] respawnStates respawnState;

    private enum directionalStates { PLAYER_DIRECTION = 0, RIGHT = 1, DOWN_RIGHT = 2, DOWN = 3, DOWN_LEFT = 4, LEFT = 5, UP_LEFT = 6, UP = 7, UP_RIGHT = 8 }
    [SerializeField] directionalStates directionState;

    public string getState() {
        if (respawnState == 0)
        {
            return "Respawns";
        } else
        {
            return "Does not respawn";
        }
    }

    public string getDirection()
    {
        switch (directionState)
        {
            case directionalStates.PLAYER_DIRECTION:
                return "Player";
            case directionalStates.RIGHT:
                return "Right";
            case directionalStates.DOWN_RIGHT:
                return "Down-right";
            case directionalStates.DOWN:
                return "Down";
            case directionalStates.DOWN_LEFT:
                return "Down-left";
            case directionalStates.LEFT:
                return "Left";
            case directionalStates.UP_LEFT:
                return "Up-left";
            case directionalStates.UP:
                return "Up";
            case directionalStates.UP_RIGHT:
                return "Up-right";
        }
        return "";
    }
}
