using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DFHack;
using System;

public class AdventureMovement : MonoBehaviour
{

    public Transform cameraCenter;

    public float repeatRate = 0.150f;
    public float repeatStart = 0.250f;

    float nexttick = 0;
    bool initialPress = true;

    public MovementOption grabPrefab;
    public MovementOption movePrefab;

    List<MovementOption> movementChoices = new List<MovementOption>();

    // Update is called once per frame
    void Update()
    {
        HandleMovementInput();
        DisplayMovementOptions();
    }

    void ClearMovementChoices()
    {
        if (movementChoices.Count == 0)
            return;
        foreach (var choice in movementChoices)
        {
            Destroy(choice.gameObject);
        }
        movementChoices.Clear();
    }

    private void DisplayMovementOptions()
    {
        if (DFConnection.Instance.AdventureMenuContents == null)
            return;

        var currentMenu = DFConnection.Instance.AdventureMenuContents;

        if (currentMenu.current_menu != AdventureControl.AdvmodeMenu.MoveCarefully)
            ClearMovementChoices();

        switch (currentMenu.current_menu)
        {
            case AdventureControl.AdvmodeMenu.Default:
                break;
            case AdventureControl.AdvmodeMenu.Look:
                break;
            case AdventureControl.AdvmodeMenu.ConversationAddress:
                break;
            case AdventureControl.AdvmodeMenu.ConversationSelect:
                break;
            case AdventureControl.AdvmodeMenu.ConversationSpeak:
                break;
            case AdventureControl.AdvmodeMenu.Inventory:
                break;
            case AdventureControl.AdvmodeMenu.Drop:
                break;
            case AdventureControl.AdvmodeMenu.ThrowItem:
                break;
            case AdventureControl.AdvmodeMenu.Wear:
                break;
            case AdventureControl.AdvmodeMenu.Remove:
                break;
            case AdventureControl.AdvmodeMenu.Interact:
                break;
            case AdventureControl.AdvmodeMenu.Put:
                break;
            case AdventureControl.AdvmodeMenu.PutContainer:
                break;
            case AdventureControl.AdvmodeMenu.Eat:
                break;
            case AdventureControl.AdvmodeMenu.ThrowAim:
                break;
            case AdventureControl.AdvmodeMenu.Fire:
                break;
            case AdventureControl.AdvmodeMenu.Get:
                break;
            case AdventureControl.AdvmodeMenu.Unk17:
                break;
            case AdventureControl.AdvmodeMenu.CombatPrefs:
                break;
            case AdventureControl.AdvmodeMenu.Companions:
                break;
            case AdventureControl.AdvmodeMenu.MovementPrefs:
                break;
            case AdventureControl.AdvmodeMenu.SpeedPrefs:
                break;
            case AdventureControl.AdvmodeMenu.InteractAction:
                break;
            case AdventureControl.AdvmodeMenu.MoveCarefully:
                if (currentMenu.movements.Count != 0 && currentMenu.movements.Count != movementChoices.Count)
                {
                    ClearMovementChoices();
                    for (int i = 0; i < currentMenu.movements.Count; i++)
                    {
                        var movementOption = currentMenu.movements[i];
                        if (movementOption.grab == null || movementOption.grab.x < 0 || (DFCoord)movementOption.grab == movementOption.dest)
                        {
                            var move = Instantiate(movePrefab,
                                GameMap.DFtoUnityTileCenter(movementOption.dest), Quaternion.identity, transform);
                            move.choiceIndex = i;
                            movementChoices.Add(move);
                        }
                        else
                        {
                            var move = Instantiate(grabPrefab,
                                (GameMap.DFtoUnityTileCenter(movementOption.dest) + GameMap.DFtoUnityTileCenter(movementOption.grab)) / 2,
                                Quaternion.LookRotation(GameMap.DFtoUnityTileCenter(movementOption.grab) - GameMap.DFtoUnityTileCenter(movementOption.dest), Vector3.up),
                                transform);
                            move.choiceIndex = i;
                            movementChoices.Add(move);
                        }
                    }
                }
                break;
            case AdventureControl.AdvmodeMenu.Announcements:
                break;
            case AdventureControl.AdvmodeMenu.UseBuilding:
                break;
            case AdventureControl.AdvmodeMenu.Travel:
                break;
            case AdventureControl.AdvmodeMenu.Unk27:
                break;
            case AdventureControl.AdvmodeMenu.Unk28:
                break;
            case AdventureControl.AdvmodeMenu.SleepConfirm:
                break;
            case AdventureControl.AdvmodeMenu.SelectInteractionTarget:
                break;
            case AdventureControl.AdvmodeMenu.Unk31:
                break;
            case AdventureControl.AdvmodeMenu.Unk32:
                break;
            case AdventureControl.AdvmodeMenu.FallAction:
                break;
            case AdventureControl.AdvmodeMenu.ViewTracks:
                break;
            case AdventureControl.AdvmodeMenu.Jump:
                break;
            case AdventureControl.AdvmodeMenu.Unk36:
                break;
            case AdventureControl.AdvmodeMenu.AttackConfirm:
                break;
            case AdventureControl.AdvmodeMenu.AttackType:
                break;
            case AdventureControl.AdvmodeMenu.AttackBodypart:
                break;
            case AdventureControl.AdvmodeMenu.AttackStrike:
                break;
            case AdventureControl.AdvmodeMenu.Unk41:
                break;
            case AdventureControl.AdvmodeMenu.Unk42:
                break;
            case AdventureControl.AdvmodeMenu.DodgeDirection:
                break;
            case AdventureControl.AdvmodeMenu.Unk44:
                break;
            case AdventureControl.AdvmodeMenu.Unk45:
                break;
            case AdventureControl.AdvmodeMenu.Build:
                break;
            default:
                break;
        }
    }

    void HandleMovementInput()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("CamUpDown"), Input.GetAxisRaw("Vertical"));
        if (moveDir.sqrMagnitude > 0.1)
        {
            nexttick -= Time.unscaledDeltaTime;
            if (nexttick <= 0 || initialPress)
            {
                moveDir = cameraCenter.TransformDirection(moveDir);

                moveDir /= Mathf.Max(Mathf.Abs(moveDir.x), Mathf.Abs(moveDir.y), Mathf.Abs(moveDir.z));

                DFCoord outDir = new DFCoord(
                    Mathf.RoundToInt(moveDir.x),
                    Mathf.RoundToInt(-moveDir.z),
                    Mathf.RoundToInt(moveDir.y)
                    );
                DFConnection.Instance.SendMoveCommand(outDir);
                if (initialPress)
                {
                    nexttick = repeatStart;
                    initialPress = false;
                }
                else
                {
                    nexttick = repeatRate;
                }
            }
        }
        else
        {
            initialPress = true;
        }

        if (Input.GetButtonDown("Jump"))
        {
            var jumpDir = cameraCenter.forward;

            jumpDir /= Mathf.Max(Mathf.Abs(jumpDir.x), Mathf.Abs(jumpDir.z));

            jumpDir *= 2;

            DFCoord outDir = new DFCoord(
                Mathf.RoundToInt(jumpDir.x),
                Mathf.RoundToInt(-jumpDir.z),
                0
                );
            DFConnection.Instance.SendJumpCommand(outDir);
        }
        if (Input.GetButtonDown("Crouch"))
        {
            DFConnection.Instance.SendMiscMoveCommand(AdventureControl.MiscMoveType.SET_STAND);
        }
        if (Input.GetButtonDown("Grab"))
        {
            DFConnection.Instance.SendMiscMoveCommand(AdventureControl.MiscMoveType.SET_CLIMB);
        }
        if (Input.GetButtonDown("Cancel"))
        {
            DFConnection.Instance.SendMiscMoveCommand(AdventureControl.MiscMoveType.SET_CANCEL);
        }
    }
}
