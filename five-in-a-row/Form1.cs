using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xDialog;

namespace five_in_a_row
{

    //棋子状态
    public enum ChessBoardState
    {
        empty = 0,
        black = 1,
        white = 2,
        outrange =3
    };
    public partial class Form1 : Form
    {
        private ChessBoardState nextstate;
        private bool GameEnable = false;
        private Point ChessLocation;
        private Rule rule=new Rule();
        private GameJudge myjudge = new GameJudge();
        private ChessBoardState mystate;
        public Form1()
        {
            InitializeComponent();
            this.nextstate = ChessBoardState.empty;
            //将下拉框设置为不可编辑状态,只能选择
            this.comboBox1.Text = this.comboBox1.Items[0].ToString();
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.textBox1.AppendText("请点击开始游戏\r\n祝您游戏愉快");
        }
        //棋盘界面初始化
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen2 = new Pen(Color.Brown, 3);
            
            for (int i = 0; i < 19; i++)
            {
                g.DrawLine(pen2, 15, 15 + i * 30, 615, 15 + i * 30);
            }
            for (int i = 0; i < 21; i++)
            {
                g.DrawLine(pen2, 15 + i * 30, 15, 15 + i * 30, 465);
            }
        }
        //开始游戏
        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox1.ResetText();
            this.textBox1.AppendText("黑子先行\n");
            if(this.comboBox1.Text=="黑子")
            {
                this.textBox1.AppendText("您执黑子\n");
                mystate = ChessBoardState.white;
                this.nextstate = ChessBoardState.black;
            }
            else
            {
                this.textBox1.AppendText("您执白子\n");
                mystate = ChessBoardState.black;
                this.nextstate = ChessBoardState.white;
                rule.SetChessBoardState(9, 10, ChessBoardState.black);
                myjudge.ChessJudgeInitSet(9, 10, ChessBoardState.black);
                ChessLocation.X = 9;
                ChessLocation.Y = 10;
                //画棋子
                Graphics g = this.pictureBox1.CreateGraphics();
                    Pen pen3 = new Pen(Color.Black, 10);
                    g.DrawEllipse(pen3, 10 + 30 * ChessLocation.X, 10 + 30 * ChessLocation.Y, 10, 10);
                g.Dispose();
            }
            this.comboBox1.Enabled = false;
            GameEnable = true;
        }
        //悔棋
        private void button2_Click(object sender, EventArgs e)
        {
            //悔棋其实是把这次存储的棋盘删掉，然后将棋盘重画，少画最后一个点
            this.textBox1.ResetText();
            this.textBox1.AppendText("只能悔棋一次哦:-)\n");
            rule.SetChessBoardState(ChessLocation.X, ChessLocation.Y, ChessBoardState.empty);
            myjudge.ChessJudgeInitSet(ChessLocation.X, ChessLocation.Y, ChessBoardState.empty);
            this.pictureBox1.Refresh();
            Graphics g = this.pictureBox1.CreateGraphics();
            for (int i=0;i<Rule.COL;i++)
            {
                for(int j = 0;j<Rule.ROW;j++)
                {
                    if (rule.GetChessBoardState(i, j) == ChessBoardState.black)
                    {
                        Pen pen3 = new Pen(Color.Black, 10);
                        g.DrawEllipse(pen3, 10 + 30 * i, 10 + 30 * j, 10, 10);
                        pen3.Dispose();
                    }
                    else if(rule.GetChessBoardState(i, j) == ChessBoardState.white)
                    {
                        Pen pen4 = new Pen(Color.Aqua, 10);
                        g.DrawEllipse(pen4, 10 + 30 * i, 10 + 30 * j, 10, 10);
                        pen4.Dispose();
                    }
                }
            }
            //下一个棋子倒退回去
            if (this.nextstate == ChessBoardState.black)
                this.nextstate = ChessBoardState.white;
            else
                this.nextstate = ChessBoardState.black;
            g.Dispose();
        }
        //退出游戏
        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //落子判断
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //只有当游戏开始之后才能落子
            if(GameEnable == true)
            {
                //通过鼠标位置判断棋子的位置
                ChessLocation.X = (e.X + 30) / 30 - 1;
                ChessLocation.Y = (e.Y + 30) / 30 - 1;
                //防止此处已经落下棋子
                if (rule.GetChessBoardState(ChessLocation.X, ChessLocation.Y) != ChessBoardState.empty)
                    return;
                //画棋子
                Graphics g = this.pictureBox1.CreateGraphics();
                if (this.nextstate == ChessBoardState.black)
                {
                    Pen pen3 = new Pen(Color.Black, 10);
                    g.DrawEllipse(pen3, 10 + 30 * ChessLocation.X, 10 + 30 * ChessLocation.Y, 10, 10);
                }
                else
                {
                    Pen pen4 = new Pen(Color.Aqua, 10);
                    g.DrawEllipse(pen4, 10 + 30 * ChessLocation.X, 10 + 30 * ChessLocation.Y, 10, 10);
                }
                //判断是否五子连珠：若五子连珠就结束
                rule.SetChessBoardState(ChessLocation.X, ChessLocation.Y, this.nextstate);
                myjudge.ChessJudgeInitSet(ChessLocation.X, ChessLocation.Y, this.nextstate);
                //Console.WriteLine("此子得分:{0}分",rule.GetPointScore(ChessLocation.X, ChessLocation.Y, this.nextstate));
                if (rule.CheckCountLIne(ChessLocation.X, ChessLocation.Y, this.nextstate) == true)
                {
                    if(this.nextstate == ChessBoardState.white)
                    {
                        this.textBox1.AppendText("\n白子胜利！\n");
                        DialogResult result = MsgBox.Show("白子胜利", "本局结束", 
                            MsgBox.Buttons.OK, MsgBox.Icon.Exclamation, MsgBox.AnimateStyle.SlideDown);
                    }
                    else
                    {
                        this.textBox1.AppendText("\n黑子胜利！\n");
                        DialogResult result = MsgBox.Show("黑子胜利", "本局结束", 
                            MsgBox.Buttons.OK, MsgBox.Icon.Exclamation, MsgBox.AnimateStyle.SlideDown);
                    }
                    for (int i = 0; i < Rule.COL; i++)
                    {
                        for (int j = 0; j < Rule.ROW; j++)
                        {
                            rule.SetChessBoardState(i, j, ChessBoardState.empty);
                            myjudge.ChessJudgeInitSet(i, j, ChessBoardState.empty);
                        }
                    }
                    this.pictureBox1.Refresh();
                    this.textBox1.ResetText();
                    this.textBox1.AppendText("请点击开始游戏");
                    this.comboBox1.Enabled = true;
                }
                else
                {
                    this.textBox1.ResetText();
                    this.textBox1.AppendText("此子得分:" +
                            myjudge.GetPointScore(ChessLocation.X, ChessLocation.Y, this.nextstate).ToString()
                            + "分\r\n");
                    Point next = myjudge.NextJudge(mystate,out int max_score);
                    rule.SetChessBoardState(next.X,next.Y,mystate);
                    myjudge.ChessJudgeInitSet(next.X, next.Y, mystate);
                    if (mystate == ChessBoardState.black)
                    {
                        Pen pen3 = new Pen(Color.Black, 10);
                        g.DrawEllipse(pen3, 10 + 30 * next.X, 10 + 30 * next.Y, 10, 10);
                    }
                    else
                    {
                        Pen pen4 = new Pen(Color.Aqua, 10);
                        g.DrawEllipse(pen4, 10 + 30 * next.X, 10 + 30 * next.Y, 10, 10);
                    }
                    g.Dispose();
                    this.textBox1.AppendText("电脑预测:" + max_score.ToString()+"分");
                    if (rule.CheckCountLIne(next.X, next.Y, mystate) == true)
                    {
                        if (mystate == ChessBoardState.white)
                        {
                            this.textBox1.AppendText("\n白子胜利！\n");
                            DialogResult result = MsgBox.Show("白子胜利", "本局结束",
                                MsgBox.Buttons.OK, MsgBox.Icon.Exclamation, MsgBox.AnimateStyle.SlideDown);
                        }
                        else
                        {
                            this.textBox1.AppendText("\n黑子胜利！\n");
                            DialogResult result = MsgBox.Show("黑子胜利", "本局结束",
                                MsgBox.Buttons.OK, MsgBox.Icon.Exclamation, MsgBox.AnimateStyle.SlideDown);
                        }
                        for (int i = 0; i < Rule.COL; i++)
                        {
                            for (int j = 0; j < Rule.ROW; j++)
                            {
                                rule.SetChessBoardState(i, j, ChessBoardState.empty);
                                myjudge.ChessJudgeInitSet(i, j, ChessBoardState.empty);
                            }
                        }
                        this.pictureBox1.Refresh();
                        this.textBox1.ResetText();
                        this.textBox1.AppendText("请点击开始游戏");
                        this.comboBox1.Enabled = true;
                    }
                        //if (this.nextstate == ChessBoardState.white)
                        //{
                        //    this.nextstate = ChessBoardState.black;
                        //    this.textBox1.AppendText("请黑方落子");
                        //}
                        //else
                        //{
                        //    this.nextstate = ChessBoardState.white;
                        //    this.textBox1.AppendText("请白方落子");
                        //}
                    }    
            }
        }
        //重新开始游戏
        private void button4_Click(object sender, EventArgs e)
        {
            GameEnable = false;
            this.pictureBox1.Refresh();
            for (int i = 0; i < Rule.COL; i++)
            {
                for (int j = 0; j < Rule.ROW; j++)
                {
                    rule.SetChessBoardState(i, j, ChessBoardState.empty);
                    myjudge.ChessJudgeInitSet(i, j, ChessBoardState.empty);
                }
            }
            this.textBox1.ResetText();
            this.textBox1.AppendText("请点击开始游戏");
        }
    }
    class Rule
    {
        public const int ROW = 18;
        public const int COL = 21;
        private ChessBoardState[,] ChessBoard;
        private int CurrentX;
        private int CurrentY;
        private ChessBoardState CurrentState;
        //通过构造函数初始化
        public Rule()
        {
            this.ChessBoard = new ChessBoardState[COL, ROW];
            this.CurrentX = -1;
            this.CurrentY = -1;
            this.CurrentState = ChessBoardState.empty;
        }
        //获取棋盘该位置的状态
        public ChessBoardState GetChessBoardState(int x, int y)
        {
            if (x < 0 || x >= COL || y < 0 || y >= ROW)
                return ChessBoardState.outrange;
            return this.ChessBoard[x, y];
        }
        //设置棋盘该位置的状态
        public bool SetChessBoardState(int x, int y, ChessBoardState state)
        {
            if (x < 0 || x >= COL || y < 0 || y >= ROW)
                return false;
            else
            {
                this.ChessBoard[x, y] = state;
                this.CurrentX = x;
                this.CurrentY = y;
                this.CurrentState = state;
                return true;
            }
        }
        public bool SetChessBoardState(Point point, ChessBoardState state)
        {
            if (point.X < 0 || point.X >= COL || point.Y < 0 || point.Y >= ROW)
                return false;
            else
            {
                this.ChessBoard[point.X, point.Y] = state;
                this.CurrentX = point.X;
                this.CurrentY = point.Y;
                this.CurrentState = state;
                return true;
            }
        }
        //检查是否已经五子连珠
        public bool CheckCountLIne(int x, int y, ChessBoardState state)
        {
            /**********************************************************
             * 原理：找到四个方向上的第一个子，从第一个子开始查找是否存在连续的五个字
             * 版本改进：while ((y - j > ROW || GetChessBoardState(x, y - j) != state) &&j < 0)
             *           由于越界的棋子都设置为空，因此查找第一个棋子的时候不需要判断是否越界
             * bug：存在隔一个子不能判断的bug;由于需要找到第一个子存在oxooooo的情况不能识别
             ***********************************************************
            int j = -4;
            while (GetChessBoardState(x, y - j) != state && j < 0)
                j += 1;
            while (GetChessBoardState(x, y - j - 1) == state)
            {
                count += 1;
                j += 1;
            }
            Console.WriteLine("垂直方向上有{0}个棋子",count);
            if (count >= 5)
                result = true;
            else
                count = 1;
            j = -4;
            //找到第一个自己的子
            
            while (GetChessBoardState(x + j, y) != state && j < 0)
                j += 1;
            while (GetChessBoardState(x + j + 1, y) == state)
            {
                count = count + 1;
                j += 1;
            }
            Console.WriteLine("水平方向上有{0}个棋子", count);
            if (count >= 5)
                result = true;
            else
                count = 1;
            j = -4;
            //    break;
            //case Rule.LineDirection.upslope:
            while ( GetChessBoardState(x + j, y - j) != state && j < 0)
                j += 1;
            while (GetChessBoardState(x + j + 1, y - j - 1) == state)
            {
                count += 1;
                j += 1;
            }
            Console.WriteLine("右下方向上有{0}个棋子", count);
            if (count >= 5)
                result = true;
            else
                count = 1;
            j = -4;
            //    break;
            //case Rule.LineDirection.downslop:
            while (GetChessBoardState(x + j, y + j) != state && j < 0)
                j += 1;
            while (GetChessBoardState(x + j + 1, y + j + 1) == state)
            {
                count += 1;
                j += 1;
            }
            Console.WriteLine("左下方向上有{0}个棋子", count);
            if (count >= 5)
                result = true;
                */
            /**********************************************************
             * 版本：第三版
             * 原理：在前后总共九个子中寻找连续数量的是否超过5
             * 未发现bug
             ***********************************************************/
            int j, count = 0;
            for (j = -4; j < 5; j++)
            {
                if (GetChessBoardState(x + j, y) == state)
                {
                    count += 1;
                    if (count >= 5)
                        return true;
                }
                else
                    count = 0;
            }
            count = 0;
            for (j = -4; j < 5; j++)
            {
                if (GetChessBoardState(x, y - j) == state)
                {
                    count += 1;
                    //Console.WriteLine("垂直方向上有{0}个棋子", count);
                    if (count >= 5)
                        return true;
                }
                else
                    count = 0;
            }
            count = 0;
            for (j = -4; j < 5; j++)
            {
                if (GetChessBoardState(x + j, y + j) == state)
                {
                    count += 1;
                    //Console.WriteLine("右下方向上有{0}个棋子", count);
                    if (count >= 5)
                        return true;
                }
                else
                    count = 0;
            }
            count = 0;
            for (j = -4; j < 5; j++)
            {
                if (GetChessBoardState(x + j, y - j) == state)
                {
                    count += 1;
                    //Console.WriteLine("左下方向上有{0}个棋子", count);
                    if (count >= 5)
                        return true;
                }
                else
                    count = 0;
            }
            return false;
        }
    }
    /*************************************************************
     * 人机规则
     * 
     * ***********************************************************/
    ////棋型估值
    class GameJudge
    {
        Rule chessjudge = new Rule();
        private const int BE_FIVE = 10000000;
        private const int ACTIVIE_FOUR = 40000;
        private const int NON_FOUR = 5000;
        private const int ACTIVIE_THREE = 5000;
        private const int NON_THREE = 300;
        private const int ACTIVE_TWO = 300;
        private const int NON_TWO = 10;
        private const int OTHER = 1;
        private const int SEARCH_RANGE = 2;

        public void ChessJudgeInitSet(int x,int y,ChessBoardState state)
        {
            chessjudge.SetChessBoardState(x, y, state);
        }
        //计算棋子的得分
        public int GetPointScore(int x,int y,ChessBoardState state)
        {
            int j, count=0,count_max = 0;
            int score = 0;
            bool non_flag = false, now_non_flag=false;
            ChessBoardState non_state;//获得相反的状态
            if (state == ChessBoardState.black)
                non_state = ChessBoardState.white;
            else
                non_state = ChessBoardState.black;
            //水平方向
            for (j = -4; j <= 4; j++)
            {
                if (chessjudge.GetChessBoardState(x + j, y) == state)
                {
                    //判断是否有一段截断了
                    if (count == 0 && (chessjudge.GetChessBoardState(x + j - 1, y) == non_state
                        || chessjudge.GetChessBoardState(x + j - 1, y) == ChessBoardState.outrange))
                        non_flag = true;
                    count += 1;
                }
                else
                {
                    //判断是否结束截断了
                    if (chessjudge.GetChessBoardState(x + j, y) == non_state
                        || chessjudge.GetChessBoardState(x + j, y) == ChessBoardState.outrange)
                        non_flag = true;
                    if (count > count_max)
                    {
                        count_max = count;
                        now_non_flag = non_flag;
                    }
                    count = 0;
                    non_flag =false;
                }   
            }
            score += ScoreJudge(count_max, now_non_flag);
            //Console.WriteLine("水平得分:{0}分", score);
            count = 0;
            count_max = 0;

            //垂直方向
            for (j = -4; j <= 4; j++)
            {
                if (chessjudge.GetChessBoardState(x, y - j) == state)
                {
                    //判断是否有一段截断了
                    if (count == 0 && (chessjudge.GetChessBoardState(x, y - j + 1) == non_state
                        || chessjudge.GetChessBoardState(x, y - j + 1) == ChessBoardState.outrange))
                        non_flag = true;
                    count += 1;
                }
                else
                {
                    //判断是否结束截断了
                    if (chessjudge.GetChessBoardState(x, y - j) == non_state
                        || chessjudge.GetChessBoardState(x, y - j) == ChessBoardState.outrange)
                        non_flag = true;
                    if (count > count_max)
                    {
                        count_max = count;
                        now_non_flag = non_flag;
                    }
                    count = 0;
                    non_flag = false;
                }
            }
            score += ScoreJudge(count_max, now_non_flag);
            //Console.WriteLine("垂直得分:{0}分", score);
            count = 0;
            count_max = 0;
            
            //右下方向
            for (j = -4; j < 5; j++)
            {
                if (chessjudge.GetChessBoardState(x + j, y + j) == state)
                {
                    if (count == 0 && (chessjudge.GetChessBoardState(x + j - 1, y + j - 1) == non_state
                        || chessjudge.GetChessBoardState(x + j - 1, y + j - 1) == ChessBoardState.outrange))
                        non_flag = true;
                    count += 1;
                }
                else
                {
                    //判断是否结束截断了
                    if (chessjudge.GetChessBoardState(x + j, y + j) == non_state
                        || chessjudge.GetChessBoardState(x + j, y + j) == ChessBoardState.outrange)
                        non_flag = true;
                    if (count > count_max)
                    {
                        count_max = count;
                        now_non_flag = non_flag;
                    }
                    count = 0;
                    non_flag = false;
                }
            }
            score += ScoreJudge(count_max, now_non_flag);
            //Console.WriteLine("右下得分:{0}分，count_max={1},non_flag={2}", score, count_max, non_flag);
            count = 0;
            count_max = 0;
            //左下方向
            for (j = -4; j < 5; j++)
            {
                if (chessjudge.GetChessBoardState(x + j, y - j) == state)
                {
                    if (count == 0 && (chessjudge.GetChessBoardState(x + j - 1, y - j + 1) == non_state
                        || chessjudge.GetChessBoardState(x + j - 1, y - j + 1) == ChessBoardState.outrange))
                        non_flag = true;
                    count += 1;
                }
                else
                {
                    if (chessjudge.GetChessBoardState(x + j, y - j) == non_state
                        || chessjudge.GetChessBoardState(x + j, y - j) == ChessBoardState.outrange)
                        non_flag = true;
                    if (count > count_max)
                    {
                        count_max = count;
                        now_non_flag = non_flag;
                    }
                    count = 0;
                    non_flag = false;
                }
            }
            score += ScoreJudge(count_max, now_non_flag);
            //Console.WriteLine("左下得分:{0}分，count_max={1},non_flag={2}", score, count_max, non_flag);
            return score;
        }
        //计算每个方向上的最大分数
        private int ScoreJudge(int count,bool flag)
        {
            switch (count)
            {
                case 5:
                    return BE_FIVE;
                case 4:
                    if (flag == false)
                        return ACTIVIE_FOUR;
                    else
                        return NON_FOUR;
                case 3:
                    if (flag == false)
                        return ACTIVIE_THREE;
                    else
                        return NON_THREE;
                case 2:
                    if (flag == false)
                        return ACTIVE_TWO;
                    else
                        return NON_TWO;
                default: return OTHER;
            }
        }
        //int[,] ScoreMapMax = new int[Rule.COL, Rule.ROW];
        //int[,] ScoreMapMin = new int[COL, ROW];
        //private Point GetBestPoint(int[,] map)
        //{
        //    int iMax = 0, jMax = 0;
        //    for (int i = 0; i < COL; i++)
        //    {
        //        for (int j = 0; j < ROW; j++)
        //        {
        //            if (map[i, j] > map[iMax, jMax])
        //            {
        //                iMax = i;
        //                jMax = j;
        //            }
        //        }
        //    }
        //    return new Point(jMax, iMax);
        //}
        //private bool[,] map = new bool[COL, ROW];
        
        //bool result = false;
        //private ChessBoardState[,] Chessjudge = new ChessBoardState[COL, ROW];
        //int evaluate(int player)
        //{
        //    return 1;
        //}
        //设置搜索范围，减小搜索的棋子数量
        bool SearchRange(int x,int y,ChessBoardState state)
        {
            if(chessjudge.GetChessBoardState(x,y)==ChessBoardState.empty)
            {
                for (int i = (x - SEARCH_RANGE); i <= (x + SEARCH_RANGE); i++)
                {
                    for (int j = (y - SEARCH_RANGE); j <= (y + SEARCH_RANGE); j++)
                        if (chessjudge.GetChessBoardState(i,j) == ChessBoardState.white
                            ||chessjudge.GetChessBoardState(i,j)==ChessBoardState.black)
                        {
                            if (i != x || j != y)
                                return true;
                        }
                }
            }
            return false;
        }
        //剪枝算法 需要区分玩家和计算机，同时玩家可以选择先走或者后走
        //int alpha_beta(int depth,ChessBoardState state,int alpha,int beta)
        //{
        //    if(depth==6||result==true)
        //    {
        //        if (result == true)
        //            return result;
        //        else
        //        {
        //            return evaluate(player) - evaluate(player ^ 1);
        //        }
        //    }
        //    ChessBoardState anti_state;
        //    if (state == ChessBoardState.black)
        //        anti_state = ChessBoardState.white;
        //    else
        //        anti_state = ChessBoardState.black;
        //    int i, j;
        //    for(i=0;i<COL;i++)
        //    {
        //        for(j=0;j<ROW;j++)
        //        {
        //            if(SearchRange(i,j,state)==true)
        //            {
        //                Chessjudge[i, j] = state;
        //                int ans = alpha_beta(depth + 1, anti_state, alpha, beta);
        //                if (ans > alpha)
        //                {
        //                    alpha = ans;
        //                    ansx = i;
        //                    ansy = j;
        //                }
        //                if(ans <beta)
        //                {
        //                    beta = ans;
        //                }
        //                if (alpha >= beta)
        //                {

        //                    return alpha;
        //                }
        //            }
        //        }
        //    }
        //}
        //先写一个简单的MAXmin
        //其实用一个数组就够了，不需要重新定义一个棋盘
        private Point[] tmp = new Point[100];
        //private Point tmp[200];
        public Point NextJudge(ChessBoardState state,out int max)
        {
            ChessBoardState anti_state;
            int cnt=0,score_tmp;
            Point best=new Point();
            int[] min = new int[200];
            max = -BE_FIVE - 1;
            for (int i=0;i<200;i++)
            {
                min[i] = BE_FIVE+1;
            }
            if (state == ChessBoardState.black)
                anti_state = ChessBoardState.white;
            else
                anti_state = ChessBoardState.black;
            int score = 0;
            for(int i=0;i<Rule.COL;i++)
            {
                for (int j = 0; j < Rule.ROW; j++)
                {
                    if (SearchRange(i, j, state) == true)
                    {
                        chessjudge.SetChessBoardState(i, j, state);
                        score_tmp = GetPointScore(i, j, state);
                        for(int m=0;m<Rule.COL;m++)
                        {
                            for(int n=0;n<Rule.ROW;n++)
                            {
                                if (SearchRange(m, n, anti_state) == true)
                                {
                                    chessjudge.SetChessBoardState(m, n, anti_state);
                                    score = score_tmp - GetPointScore(m, n, anti_state);
                                    if(score<min[cnt])
                                    {
                                        //对方一定会取最小值,寻找最小值，并记录坐标
                                        tmp[cnt].X = i;
                                        tmp[cnt].Y = j;
                                        min[cnt] = score;
                                    }
                                    chessjudge.SetChessBoardState(m, n, ChessBoardState.empty);
                                }
                            }
                        }
                        chessjudge.SetChessBoardState(i, j, ChessBoardState.empty);
                        cnt++;
                    }
                }
            }
            cnt--;
            while (cnt >= 0)
            {
                if (min[cnt] > max)
                {
                    max = min[cnt];
                    best.X = tmp[cnt].X;
                    best.Y = tmp[cnt].Y;
                }
                cnt--;
            }
            return best;
        }
    }
}
