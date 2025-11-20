using UnityEngine.UI;

public class TestChildItem : RecyclingListViewItem
{
    public Text titleText;
    public Text rowText;

    private TestChildData childData;
    public TestChildData ChildData
    {
        get { return childData; }
        set
        {
            childData = value;
            titleText.text = childData.Title;
            rowText.text = $"行号：{childData.Row}";
        }
    }
}


public struct TestChildData
{
    public string Title;
    public int Row;

    public TestChildData(string title, int row)
    {
        Title = title;
        Row = row;
    }
}
