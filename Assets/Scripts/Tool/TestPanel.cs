using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using DafultScript;
public class TestPanel : MonoBehaviour
{
    public RecyclingListView scrollList;
    /// <summary>
    /// 列表数据
    /// </summary>
    private List<TestChildData> data = new List<TestChildData>();

    public InputField createRowCntInput;
    public Button createListBtn;
    public Button clearListBtn;

    public InputField deleteItemInput;
    public Button deleteItemBtn;
    public Button addItemBtn;

    public InputField moveToRowInput;
    public Button moveToTopBtn;
    public Button moveToCenterBtn;
    public Button moveToBottomBtn;

    private void Start()
    {
        // 列表item更新回调
        scrollList.ItemCallback = PopulateItem;

        // 创建列表
        createListBtn.onClick.AddListener(CreateList);

        // 清空列表
        clearListBtn.onClick.AddListener(ClearList);


        // 删除某一行
        deleteItemBtn.onClick.AddListener(DeleteItem);

        // 添加行
        addItemBtn.onClick.AddListener(AddItem);

        // 将目标行移动到列表的顶部、中心、底部
        moveToTopBtn.onClick.AddListener(() =>
        {
            MoveToRow(RecyclingListView.ScrollPosType.Top);
        });

        moveToCenterBtn.onClick.AddListener(() =>
        {
            MoveToRow(RecyclingListView.ScrollPosType.Center);
        });

        moveToBottomBtn.onClick.AddListener(() =>
        {
            MoveToRow(RecyclingListView.ScrollPosType.Bottom);
        });
    }

    /// <summary>
    /// item更新回调
    /// </summary>
    /// <param name="item">复用的item对象</param>
    /// <param name="rowIndex">行号</param>
    private void PopulateItem(RecyclingListViewItem item, int rowIndex)
    {
        var child = item as TestChildItem;
        child.ChildData = data[rowIndex];
    }

    private void CreateList()
    {
        if (string.IsNullOrEmpty(createRowCntInput.text))
        {
            Debug.LogError("请输入行数");
            return;
        }
        var rowCnt = int.Parse(createRowCntInput.text);

        data.Clear();
        // 模拟数据
        string[] randomTitles = new[] {
            "黄沙百战穿金甲，不破楼兰终不还",
            "且将新火试新茶，诗酒趁年华",
            "苟利国家生死以，岂因祸福避趋之",
            "枫叶经霜艳，梅花透雪香",
            "夏虫不可语于冰",
            "落花无言，人淡如菊",
            "宠辱不惊，闲看庭前花开花落；去留无意，漫随天外云卷云舒",
            "衣带渐宽终不悔，为伊消得人憔悴",
            "从善如登，从恶如崩",
            "欲穷千里目，更上一层楼",
            "草木本无意，荣枯自有时",
            "纸上得来终觉浅，绝知此事要躬行",
            "不是一番梅彻骨，怎得梅花扑鼻香",
            "青青子衿，悠悠我心",
            "瓜田不纳履，李下不正冠"
        };
        for (int i = 0; i < rowCnt; ++i)
        {
            data.Add(new TestChildData(randomTitles[Random.Range(0, randomTitles.Length)], i));
        }

        // 设置数据，此时列表会执行更新
        scrollList.RowCount = data.Count;
    }

    private void ClearList()
    {
        data.Clear();
        scrollList.Clear();
    }

    private void DeleteItem()
    {
        if (string.IsNullOrEmpty(deleteItemInput.text))
        {
            Debug.LogError("请输入行号");
            return;
        }
        var rowIndex = int.Parse(deleteItemInput.text);
        data.RemoveAll(item => (item.Row == rowIndex));

        scrollList.RowCount = data.Count;
    }

    private void AddItem()
    {
        data.Add(new TestChildData("我是新增的行", data.Count));
        scrollList.RowCount = data.Count;
    }

    private void MoveToRow(RecyclingListView.ScrollPosType posType)
    {
        if (string.IsNullOrEmpty(moveToRowInput.text))
        {
            Debug.LogError("请输入行号");
            return;
        }
        var rowIndex = int.Parse(moveToRowInput.text);
        scrollList.ScrollToRow(rowIndex, posType);
    }
}
