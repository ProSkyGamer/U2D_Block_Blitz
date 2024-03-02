using System;
using UnityEngine;

public class BuildingTetrisFigure : Figure
{
    private void Start()
    {
        BuildingTetrisUI.OnRotatePieceQ += TetrisMinigameController_OnRotatePieceQ;
        BuildingTetrisUI.OnRotatePieceE += TetrisMinigameController_OnRotatePieceE;
        BuildingTetrisUI.OnMovePieceRight += TetrisMinigameControllerOnOnMovePieceRight;
        BuildingTetrisUI.OnMovePieceLeft += TetrisMinigameController_OnMovePieceLeft;
        BuildingTetrisUI.OnMovePieceDown += TetrisMinigameController_OnMovePieceDown;
        BuildingTetrisUI.OnDropPiece += TetrisMinigameController_OnDropPiece;
    }

    private void TetrisMinigameController_OnDropPiece(object sender, EventArgs e)
    {
        HardDrop();
    }

    private void TetrisMinigameController_OnMovePieceDown(object sender, EventArgs e)
    {
        Step();
    }

    private void TetrisMinigameController_OnMovePieceLeft(object sender, EventArgs e)
    {
        Move(Vector2Int.left);
    }

    private void TetrisMinigameControllerOnOnMovePieceRight(object sender, EventArgs e)
    {
        Move(Vector2Int.right);
    }

    private void TetrisMinigameController_OnRotatePieceE(object sender, EventArgs e)
    {
        Rotate(1);
    }

    private void TetrisMinigameController_OnRotatePieceQ(object sender, EventArgs e)
    {
        Rotate(-1);
    }
}
