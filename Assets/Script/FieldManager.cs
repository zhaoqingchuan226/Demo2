using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class cardDialog
{
    public int id;
    public float pro_do;//对话触发的概率，1为必然触发
    public float pro_des;//对话触发后被摧毁的概率，被摧毁后再也不出现，也就是只触发一次的对话，pro_des=1
    public string s;
    public string situation;//什么时候触发
    public cardDialog(int id1, float pro_do1, float pro_des1, string s1, string situation1)
    {
        id = id1;
        pro_do = pro_do1;
        pro_des = pro_des1;
        s = s1;
        situation = situation1;
    }
}
//这个单例是用来统计各种奇奇怪怪的，会被卡牌的condi或者func用到的属性的
public class FieldManager : MonoSingleton<FieldManager>
{
    //卡牌区
    [HideInInspector]
    public bool isTripletReward = false;

    [HideInInspector]
    public int luckyTimes = 0; //幸运的次数,如果>0，那么本周末的三选一面板中必有三张下一等级的卡牌//由func F 开启

    [HideInInspector]
    public int TiredAll = 0; //场上总疲劳数

    [HideInInspector]
    public int NoColorAll = 0; //场上总无色卡数

    [HideInInspector]
    public int NoneAll = 20; //场上总空格数

    [HideInInspector]
    public int TypeCount = 0; //场上卡牌流派数

    [HideInInspector]
    public int universe = 0; //宇宙加成，数值为1时则表面是宇宙

    [HideInInspector]
    public int FishAll = 0; //场上带有“摸鱼”关键词的卡牌数

    public int Lv5C = 0; //场上质量等级5级的卡牌数

    [HideInInspector]
    public int FishPeopleAll = 0; //场上带有“摸鱼”关键词的卡牌数

    [HideInInspector]
    public int ET18 = 0; //id18的统筹把控卡牌总共吃掉过多少一级红卡牌
    public int ET42 = 0; //id42的怨恨执行卡牌总共吃掉过多少一级红卡牌
    public int DestroyCard = 0;//摧毁的卡牌数


    [HideInInspector]
    public int OverFillP = 0; //每回合P的溢出

    [HideInInspector]
    public int OverFillS = 0; //每回合S的溢出

    [HideInInspector] public int Ka1AIKa1 = 0;//创造KPI的能力 第一的AI的 创造KPI的能力
    [HideInInspector] public int KPIAll = 0;//上一月KPI的总量

    [HideInInspector] public List<AIData> AIDatas_Remove = new List<AIData>();//需要抹除的AIData


    //桌面配置区
    [HideInInspector] public bool isUnDeath = false;//招魂仪的不死效果;
    [HideInInspector] public bool isOverload = false;//提神发夹的增加消耗，增加收益;

    [HideInInspector] public bool isFlower_Overload = false;//血蔷薇，降低白天消耗，增加晚上收益;
    [HideInInspector] public bool isBook_Cthugha = false;//禁忌书，月末给卡;
    [HideInInspector] public bool isMouse = false;//高手的鼠标;
     [HideInInspector] public bool isKnife_HuiJie = false;//绘阶刀;
    [HideInInspector] public int Counter_HuiJie = 0;//绘阶刀计数;
    [HideInInspector] public int Level_HuiJie = 0;//绘阶刀等级，累积五次计数后，计数清零，等级+1;
    //棋子台词区
    [HideInInspector] public Dictionary<int, List<cardDialog>> cardID_cardDialogs_Dic = new Dictionary<int, List<cardDialog>>();


    [HideInInspector] public bool isSpeakIn1_3 = false;//三选一面板只能有一只棋子说话，不然就会过多
    void SetDic(int id, float pro_do, float pro_des, string s, string situation)
    {
        if (cardID_cardDialogs_Dic.ContainsKey(id))
        {
            cardID_cardDialogs_Dic[id].Add(new cardDialog(id, pro_do, pro_des, s, situation));
        }
        else
        {
            cardID_cardDialogs_Dic.Add(id, new List<cardDialog> { new cardDialog(id, pro_do, pro_des, s, situation) });
        }

    }

    void Awake()
    {
        LoadDialog();

    }
    // public void PlayCardDialog(int id, string situation)
    // {
    //     if (cardID_dialogs_Dic[id].Count > 0)
    //     {
    //         for (int i = 0; i < cardID_dialogs_Dic[id].Count; i++)
    //         {
    //             if (cardID_dialogs_Dic[id][i].situation == situation)
    //             {

    //                 break;//一次只播一段
    //             }
    //         }
    //     }
    // }


    void LoadDialog()
    {
        //要求，一个id下，一定要先写只出现一次的台词
        //再写会多次出现的台词
        //三局随机台词
        SetDic(1, 1f, 1f, "工作，工作", "chess");
        SetDic(1, 0.1f, 0, "我不累", "chess");
        SetDic(1, 0.1f, 0, "我还能干", "chess");
        SetDic(1, 0.1f, 0, "放我出去", "chess");
        SetDic(1, 0.1f, 0, "我曾也是打工人", "chess");
        SetDic(1, 0.1f, 0, "我曾也是资本家", "chess");
        SetDic(1, 0.2f, 0, "旁边的，你怎么不反抗", "1_3");
        SetDic(1, 0.2f, 0, "我们扳手，都是一类人", "1_3");
        SetDic(1, 0.2f, 0, "虽然都是扳手，但我们几乎没有联系", "1_3");

        SetDic(2, 0.5f, 1f, "开启夜晚时代，准备好了吗", "1_3");
        SetDic(2, 0.1f, 0f, "她晚上一直都在", "chess");
        SetDic(2, 0.1f, 0f, "加班都是人上人", "chess");
        SetDic(2, 0.1f, 0f, "夜和你，只能熬过去一个", "chess");
        SetDic(2, 0.1f, 0f, "为了加班工资！", "chess");

        SetDic(5, 0.5f, 1f, "强调一下，椪糖不只属于韩国", "1_3");

        SetDic(6, 0.5f, 1f, "重复就是真理", "1_3");
        SetDic(6, 0.1f, 0, "重复，重复", "chess");
        SetDic(6, 0.1f, 0, "循环，循环", "chess");

        SetDic(7, 0.5f, 1f, "感受打擦边球的乐趣吧，和死亡", "1_3");


        SetDic(8, 0.5f, 1f, "杀人还要诛心，好可怕呀！", "1_3");
        SetDic(8, 1f, 0, "Cthugha！Cthugha！", "chess");

        SetDic(9, 0.5f, 1f, "我就是个祭吧", "1_3");
        SetDic(9, 1f, 0, "Cthugha！Cthugha！", "chess");

        SetDic(10, 0.5f, 1f, "大概就是带头冲锋", "1_3");
        SetDic(10, 1f, 0, "Cthugha！Cthugha！", "chess");

        SetDic(11, 0.5f, 1f, "只有聪明人，配得上我", "1_3");
        SetDic(11, 0.1f, 0, "啊......好吵", "chess");
        SetDic(11, 0.1f, 0, "人聪明，话就不会多", "chess");
        SetDic(11, 0.1f, 0, "你，颇有远见", "chess");
        SetDic(11, 0.1f, 0, "一味的勤奋，堪称愚蠢", "chess");
        SetDic(11, 0.1f, 0, "能力为王", "chess");
        SetDic(11, 0.2f, 0, "没有实力的家伙才会去靠人际", "chess");

        SetDic(12, 0.5f, 1, "把我和这种家伙绑在一起，我很难受", "1_3");
        SetDic(12, 0.1f, 0, "呕1", "chess");
        SetDic(12, 0.1f, 0, "太难受了！", "chess");
        SetDic(12, 0.1f, 0, "分开我们！", "chess");
        SetDic(12, 0.1f, 0, "别拧我的脑子！", "chess");

        SetDic(13, 0.5f, 1, "芜湖出品", "1_3");
        SetDic(13, 1f, 1f, "哎，起飞！", "chess");

        SetDic(14, 0.5f, 1, "我是更好的扳手", "1_3");
        SetDic(14, 0.1f, 0, "绿色的力量，牛头人的力量", "chess");
        SetDic(14, 0.1f, 0, "击碎纯爱战士", "chess");
        SetDic(14, 0.1f, 0, "爱和我，是一样的颜色", "chess");

        SetDic(15, 0.5f, 1, "来呀，姐妹", "1_3");
        SetDic(15, 0.1f, 0, "紫色，多么高贵！", "chess");
        SetDic(15, 0.1f, 0, "见到你后，我无意地柔情万种", "chess");
        SetDic(15, 0.1f, 0, "我们或许是灵魂同志呢", "chess");
        SetDic(15, 0.1f, 0, "当初被封印时，我特意选了这个颜色", "chess");

        SetDic(16, 0.5f, 1, "你找不到比我更猛的了", "1_3");
        SetDic(16, 0.1f, 0, "静下心来，万事皆成", "chess");
        SetDic(16, 0.1f, 0, "我很强，你留不住我", "chess");
        SetDic(16, 0.1f, 0, "不持久，也许是我唯一的弱点", "chess");
        SetDic(16, 0.1f, 0, "那条狗，和我有一样的病症", "chess");

        SetDic(17, 0.5f, 1, "据说21世纪，有一个很懂我的歌星，你认识吗？", "1_3");

        SetDic(18, 0.5f, 1, "消灭那些垃圾行为，让我变强！", "1_3");
        SetDic(18, 0.1f, 0, "吃掉，吃掉！", "chess");
        SetDic(18, 0.1f, 0, "我饿了！", "chess");
        SetDic(18, 0.1f, 0, "弱者，被我吃掉吧！", "chess");
        SetDic(18, 0.1f, 0, "消化！废物！", "chess");

        SetDic(19, 0.5f, 1, "精雕细琢，臻于完美", "1_3");
        SetDic(19, 0.1f, 0, "只有蒸汽零件的连结艺术，才配得上 专精 二字", "chess");
        SetDic(19, 0.1f, 0, "只干一行，只爱一行", "chess");

        SetDic(20, 0.5f, 1, "别选我，让我静静", "1_3");
        SetDic(20, 0.1f, 0, "一个人，只要我一个人就够了", "chess");
        SetDic(20, 0.1f, 0, "别妨碍我，周围的杂碎", "chess");
        SetDic(20, 0.1f, 0, "啊......这里好幽闭......我好喜欢......", "chess");

        SetDic(21, 1f, 1f, "来吧，摸一摸我", "1_3");
        SetDic(21, 0.1f, 0f, "找到组织，我们会更强大", "1_3");
        SetDic(21, 0.1f, 0f, "我的伙伴去哪了", "1_3");
        SetDic(21, 0.1f, 0f, "加入我们", "1_3");
        SetDic(21, 0.1f, 0f, "独鱼不成林", "1_3");
        SetDic(21, 0.2f, 0f, "带领我们鱼人走向辉煌", "1_3");
        SetDic(21, 0.2f, 0f, "7秒前发生了什么", "1_3");
        SetDic(21, 0.2f, 0f, "再来一条，再来一条", "1_3");
        SetDic(21, 1f, 0f, "为了鱼人", "chess");

        SetDic(22, 0.5f, 1, "磨刀不误砍柴工，中午的时间绝不可能留给工作", "1_3");

        SetDic(23, 0.5f, 1, "三点几嚟，饮茶先啦!", "1_3");
        SetDic(23, 0.1f, 0, "做做做做撚啊做!", "chess");
        SetDic(23, 0.1f, 0, "做咁多都冇用!", "chess");
        SetDic(23, 0.1f, 0, "老細唔錫你啦喂！", "chess");
        SetDic(23, 0.1f, 0, "做碌鳩啊做！", "chess");

        SetDic(24, 0.5f, 1, "身体健康的选我！", "1_3");

        SetDic(25, 0.5f, 0f, "上班时间，最重要的就是养生，不会真有人喜欢工作吧？", "1_3");

        SetDic(26, 0.5f, 0f, "你觉得那些摸鱼的人是怎么挣到绩效的，嗯？", "1_3");
        SetDic(26, 1f, 0, "Cthulhu！Cthulhu！", "chess");

        SetDic(27, 0.5f, 0f, "摸鱼的真正爽点就在于：别人卷，就你摸", "1_3");
        SetDic(27, 0.1f, 0, "你们接着卷，我已经躺平了", "chess");
        SetDic(27, 0.1f, 0, "这可不是什么幸灾乐祸", "chess");
        SetDic(27, 0.1f, 0, "啊，不要停，不要停，卷王们", "chess");

        SetDic(28, 0.5f, 1f, "一起去上厕所吗，我们一排", "1_3");
        SetDic(28, 1f, 0, "为了鱼人", "chess");

        SetDic(29, 0.5f, 1f, "健身吗，我教你", "1_3");
        SetDic(29, 1f, 0, "为了鱼人", "chess");

        SetDic(30, 0.5f, 1f, "和领导做好交易，和大家处好关系，你就可以开心地摸了", "1_3");
        SetDic(30, 1f, 0, "为了鱼人", "chess");

        SetDic(31, 0.5f, 1f, "是时候摆人一道了！", "1_3");
        SetDic(31, 0.1f, 0, "这是为别人好，当然，最重要的是为你好", "chess");
        SetDic(31, 0.1f, 0, "偶尔举报一下，一周绝不超过两次！", "chess");
        SetDic(31, 0.1f, 0, "清除所有的异己", "chess");

        SetDic(32, 0.5f, 1f, "掠夺！", "1_3");
        SetDic(32, 1f, 0f, "你的绩效归我了", "chess");

        SetDic(33, 0.5f, 1f, "不必亲自动手，我来教你借刀杀人！", "1_3");
        SetDic(33, 1f, 0f, "拿起搅屎棍，我们动手吧！", "chess");

        SetDic(35, 0.5f, 1f, "我前辈的最高战绩，就是毒死过一位董事长", "1_3");
        SetDic(35, 0.1f, 0f, "开盖有奖，再来一瓶", "chess");

        SetDic(36, 0.5f, 1f, "我说我都能干，领导当真了", "1_3");
        SetDic(36, 1f, 0f, "给你两张牌，祝你好运", "chess");


        SetDic(37, 0.5f, 1f, "嘘......你想对付和你关系差的同事吗", "1_3");

        SetDic(38, 0.5f, 1f, "害一个人还需要什么理由吗", "1_3");

        SetDic(39, 0.5f, 1f, "KPI要求太高了对不对，我来帮你，嘿嘿嘿", "1_3");

        SetDic(40, 0.5f, 1f, "舔领导舔到最后当然是应有尽有", "1_3");

        SetDic(41, 0.5f, 1f, "老子不是好惹的", "1_3");
        SetDic(41, 0.2f, 0f, "来干我啊", "chess");
        SetDic(41, 0.2f, 0f, "你越针对我，我就越强大", "chess");

        SetDic(42, 0.5f, 1f, "我们已然承受太多，孩子", "1_3");
        SetDic(42, 0.3f, 0f, "你的苦难将由我们消除", "chess");
        SetDic(42, 0.3f, 0f, "怨恨将吞噬你的苦痛", "chess");

        SetDic(43, 0.5f, 1f, "我会让你以一种新的视角去看坏事", "1_3");
        SetDic(43, 1f, 0f, "所有的非议，都是收获", "chess");

        SetDic(44, 0.5f, 1f, "Naive！", "1_3");

        SetDic(45, 0.5f, 1f, "看看，那个同事在干什么？", "1_3");
        SetDic(45, 1f, 0, "拷贝！拷贝！", "1_3");

        SetDic(46, 0.5f, 1f, "看看，大家都在干什么？", "1_3");
        SetDic(46, 1f, 0, "拷贝！拷贝！", "1_3");

        SetDic(47, 0.5f, 1f, "啊......混乱......随机......", "1_3");
        SetDic(47, 1f, 0, "升级吧，变成什么都好", "chess");

        SetDic(48, 0.5f, 1f, "所有得罪过你的人，我都会帮你记下", "1_3");
        SetDic(48, 0.2f, 0, "啊，是你逼我的", "chess");
        SetDic(48, 0.2f, 0, "如此......可恨", "chess");
        SetDic(48, 0.2f, 0, "恶毒的人，恶毒的心", "chess");
        SetDic(48, 0.2f, 0, "真是......乌烟瘴气", "chess3");
        SetDic(48, 1f, 0, "我快要受够了", "chess");

        SetDic(49, 0.5f, 1f, "我在找一个本子，和我差不多大，你看见过它吗", "1_3");
        SetDic(49, 1f, 0, "血债，终于在这一刻清算！", "chess");

        SetDic(50, 0.5f, 1f, "吭哧吭哧", "1_3");
        SetDic(50, 0.2f, 0, "啊，和我无关", "chess");
        SetDic(50, 0.2f, 0, "别和我扯上关系", "chess");

        SetDic(51, 1f, 1f, "你选择的不是卡牌，而是决定他人命运的权力！", "1_3");
        SetDic(51, 0.2f, 0, "每次属下有收获，那都是我的功劳", "chess");

        SetDic(52, 0.5f, 1f, "让我们共铸辉煌", "1_3");
        SetDic(52, 0.2f, 0, "加班吧，希望就在明天", "chess");

        SetDic(53, 0.5f, 1f, "作为表率，义不容辞", "1_3");
        SetDic(53, 0.2f, 0, "只是做做样子", "chess");

        SetDic(54, 1f, 1f, "至暗时刻终将成为过往，愿意随我前行的人们，保护好你右手边的战友", "1_3");
        SetDic(54, 0.2f, 0, "所以接下来该怎么办", "chess");

        SetDic(55, 0.5f, 1f, "汇报工作的时间到了", "1_3");

        SetDic(56, 0.5f, 1f, "资本家卷，打工人获益，反过来呢？", "1_3");

        SetDic(57, 0.5f, 1f, "你曾觉得画饼的领导不是人？现在的你们，没有什么不同", "1_3");
        SetDic(57, 0.15f, 0, "公司的未来，就靠你了", "chess");
        SetDic(57, 0.15f, 0, "在我们这里，一年拿到300k不成问题", "chess");
        SetDic(57, 0.15f, 0, "项目完成后，每个人都有分红", "chess");
        SetDic(57, 0.15f, 0, "公司明年要开分部了，我觉得你适合当负责人", "chess");
        SetDic(57, 0.15f, 0, "你隔壁的比你早来半年，月薪已经过这个数了", "chess");
        SetDic(57, 1f, 0, "我们一旦上市，年入百万将不是梦", "chess");

        SetDic(58, 1f, 1f, "你画的饼是不是快过期了？", "1_3");
        SetDic(58, 1f, 0f, "续！", "1_3");

        SetDic(59, 0.5f, 1f, "干掉那个业绩最差的人！", "1_3");
        SetDic(59, 1f, 0f, "莫怪我，这就是物竞天择", "1_3");

        SetDic(60, 0.5f, 1f, "干掉那个身体最差的人！", "1_3");
        SetDic(60, 1f, 0f, "赔礼道歉也行，但我们不需要身体差的员工", "1_3");

        SetDic(61, 0.5f, 1f, "高贵，像我一样高贵！", "1_3");

        SetDic(62, 0.5f, 1f, "那些你经历过的曲折，终将成为你的财富", "1_3");

        SetDic(63, 0.5f, 1f, "现实中沉重的引力会让任何超脱飞扬的思想砰然坠地", "1_3");

        SetDic(71, 0.5f, 1f, "万金油，万金油！", "1_3");

        SetDic(72, 0.5f, 1f, "宇宙中的星辰，没有一颗是重复的", "1_3");





        SetDic(10000, 0.1f, 0, "疲劳了，什么都干不了", "chess");
        SetDic(10000, 0.1f, 0, "总是因人际和斗争而劳累", "chess");
        SetDic(10000, 0.1f, 0, "腰酸背痛，领导训我我也要休息", "chess");

        SetDic(10001, 0.1f, 0, "被举报了", "chess");
        SetDic(10001, 0.1f, 0, "你要陷害我是吧？", "chess");
        SetDic(10001, 0.1f, 0, "是谁要陷害我呀？", "chess");

        SetDic(10002, 0.1f, 0, "不就是鸡汤吗，我喝，我喝", "chess");
        SetDic(10002, 0.1f, 0, "这喝汤，多是一件美事", "chess");
        SetDic(10002, 0.1f, 0, "忍住，肚子好疼", "chess");

        SetDic(10003, 0.1f, 0, "今天我还就和你杠上了！", "chess");
        SetDic(10003, 0.1f, 0, "班都不上了我也要教训他一顿！", "chess");
        SetDic(10003, 0.1f, 0, "踢凹垃圾筒，我系地上最强", "chess");

        SetDic(10003, 0.2f, 0, "他和她约会啵嘴，我留下加班开会", "chess");
    }

}
