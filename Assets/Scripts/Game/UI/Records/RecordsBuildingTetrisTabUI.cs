using System;

public class RecordsBuildingTetrisTabUI : RecordsTabBaseUI
{
    private void Start()
    {
        InfoUI.OnRecordsBuildingTetrisTabOpened += InfoUI_OnRecordsBuildingTetrisTabOpened;
        InfoUI.OnOtherTabOpened += InfoUI_OnOtherTabOpened;
    }

    private void InfoUI_OnOtherTabOpened(object sender, EventArgs e)
    {
        Hide();
    }

    private void InfoUI_OnRecordsBuildingTetrisTabOpened(object sender, EventArgs e)
    {
        Show();
    }
}
