using System;
using UnityEngine;

public class BuildingSuikaFigure : Figure
{
    private void Start()
    {
        SuikaMinigameController.OnRotatePieceQ += TetrisMinigameController_OnRotatePieceQ;
        SuikaMinigameController.OnRotatePieceE += TetrisMinigameController_OnRotatePieceE;
        SuikaMinigameController.OnMovePieceRight += TetrisMinigameControllerOnOnMovePieceRight;
        SuikaMinigameController.OnMovePieceLeft += TetrisMinigameController_OnMovePieceLeft;
        SuikaMinigameController.OnDropPiece += TetrisMinigameController_OnDropPiece;
    }

    private void TetrisMinigameController_OnDropPiece(object sender, EventArgs e)
    {
        HardDrop();
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
