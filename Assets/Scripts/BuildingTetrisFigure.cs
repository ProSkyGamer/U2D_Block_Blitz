using System;
using UnityEngine;

public class BuildingTetrisFigure : Figure
{
    private void Start()
    {
        TetrisMinigameController.OnRotatePieceQ += TetrisMinigameController_OnRotatePieceQ;
        TetrisMinigameController.OnRotatePieceE += TetrisMinigameController_OnRotatePieceE;
        TetrisMinigameController.OnMovePieceRight += TetrisMinigameControllerOnOnMovePieceRight;
        TetrisMinigameController.OnMovePieceLeft += TetrisMinigameController_OnMovePieceLeft;
        TetrisMinigameController.OnMovePieceDown += TetrisMinigameController_OnMovePieceDown;
        TetrisMinigameController.OnDropPiece += TetrisMinigameController_OnDropPiece;
    }

    private void TetrisMinigameController_OnDropPiece(object sender, EventArgs e)
    {
        HardDrop();
    }

    private void TetrisMinigameController_OnMovePieceDown(object sender, EventArgs e)
    {
        Move(Vector2Int.down);
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
