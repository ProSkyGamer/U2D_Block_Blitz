using System;

public class RecordsSameGameTabUI : RecordsTabBaseUI
{
    private void Start()
    {
        InfoUI.OnRecordsSameGameTabOpened += InfoUI_OnRecordsSameGameTabOpened;
        InfoUI.OnOtherTabOpened += InfoUI_OnOtherTabOpened;
    }

    private void InfoUI_OnOtherTabOpened(object sender, EventArgs e)
    {
        Hide();
    }

    private void InfoUI_OnRecordsSameGameTabOpened(object sender, EventArgs e)
    {
        Show();
    }
}
