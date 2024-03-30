using System;

public class RecordsSuikaTetrisTabUI : RecordsTabBaseUI
{
    private void Start()
    {
        InfoUI.OnRecordsSuikaTetrisTabOpened += InfoUI_OnRecordsSuikaTetrisTabOpened;
        InfoUI.OnOtherTabOpened += InfoUI_OnOtherTabOpened;
    }

    private void InfoUI_OnOtherTabOpened(object sender, EventArgs e)
    {
        Hide();
    }

    private void InfoUI_OnRecordsSuikaTetrisTabOpened(object sender, EventArgs e)
    {
        Show();
    }
}
