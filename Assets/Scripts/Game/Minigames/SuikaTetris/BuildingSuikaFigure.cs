using System;
using UnityEngine;

public class BuildingSuikaFigure : Figure
{
    private void Start()
    {
        SuikaTetrisUI.OnRotatePieceQ += TetrisMinigameController_OnRotatePieceQ;
        SuikaTetrisUI.OnRotatePieceE += TetrisMinigameController_OnRotatePieceE;
        SuikaTetrisUI.OnMovePieceRight += TetrisMinigameControllerOnOnMovePieceRight;
        SuikaTetrisUI.OnMovePieceLeft += TetrisMinigameController_OnMovePieceLeft;
        SuikaTetrisUI.OnDropPiece += TetrisMinigameController_OnDropPiece;
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
