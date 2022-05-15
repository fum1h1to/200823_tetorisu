using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

public class Tetorisu : Form{
    static readonly int TILE_SIZE = 12;
    static readonly int TIMER_INTERVAL = 16;
    static readonly int MAP_WIDTH = 14;//座標単位
    static readonly int MAP_HEIGHT = 25;
    static readonly int SCR_WIDTH = MAP_WIDTH * TILE_SIZE;//仮想画面。今回は拡大するために使用
    static readonly int SCR_HEIGHT = MAP_HEIGHT * TILE_SIZE;
    static readonly int WND_WIDTH = SCR_WIDTH * 2;
    static readonly int WND_HEIGHT = SCR_HEIGHT * 2;
    static readonly int WAIT = 60;//frame
    
    static readonly byte[,,] mBlock = {
        {   
            {0, 0, 0, 0},
            {1, 1, 1, 1},
            {0, 0, 0, 0},
            {0, 0, 0, 0},
        },{   
            {0, 0, 0, 0},
            {0, 1, 1, 1},
            {0, 1, 0, 0},
            {0, 0, 0, 0},
        },{   
            {0, 0, 0, 0},
            {0, 1, 1, 0},
            {0, 1, 1, 0},
            {0, 0, 0, 0},
        },{   
            {0, 0, 0, 0},
            {1, 1, 0, 0},
            {0, 1, 1, 0},
            {0, 0, 0, 0},
        },{   
            {0, 0, 0, 0},
            {1, 1, 1, 0},
            {0, 1, 0, 0},
            {0, 0, 0, 0},
        },{   
            {0, 0, 0, 0},
            {1, 1, 1, 0},
            {0, 0, 1, 0},
            {0, 0, 0, 0},
        },{   
            {0, 0, 0, 0},
            {0, 1, 1, 0},
            {1, 1, 0, 0},
            {0, 0, 0, 0},
        }
    };//多次元配列の考え方は、配列の配列を作れるという考え方。三次元は二次元配列の配列

    public byte[,] mField = {
        {9, 9, 9, 9, 8, 7, 7, 7, 7, 8, 9, 9, 9, 9},
        {9, 9, 9, 9, 8, 7, 7, 7, 7, 8, 9, 9, 9, 9},
        {9, 8, 8, 8, 8, 7, 7, 7, 7, 8, 8, 8, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 9},
        {9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9}
    };//多次元配列の指定方法は二次元だったらint[,]、三次元だったら[, ,]だから注意

    public byte[,] InitField = {
        {9, 9, 9, 9, 8, 7, 7, 7, 7, 8, 9, 9, 9, 9},
        {9, 9, 9, 9, 8, 7, 7, 7, 7, 8, 9, 9, 9, 9},
        {9, 8, 8, 8, 8, 7, 7, 7, 7, 8, 8, 8, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 8, 9},
        {9, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 9},
        {9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9}
    };
    Bitmap mScreen = new Bitmap(SCR_WIDTH, SCR_HEIGHT);//空のImageを作成。仮想画面になる
    Bitmap[] mTile;

    int mX = 5, mY = 3, mA, mWait;
    byte mT = 0, mNext;
    int mKeyL, mKeyR, mKeyA, mKeyD;
    Random mRnd = new Random();
    bool mGameOver = false;
    int screen = 0;
    
    protected override void OnKeyDown(KeyEventArgs e){
        //押し続けて連続になるまでに少し間がある。それはＯＳの初期設定。だがその間にもtaskの中のwhileは実行されているから、その間だけmKeyLとかの変数がゼロにならなかった。
        if(e.KeyCode == Keys.Left ) mKeyL++;
        if(e.KeyCode == Keys.Right) mKeyR++;
        if(e.KeyCode == Keys.D    ) mKeyD++;
        if(e.KeyCode == Keys.A    ) mKeyA++;
    }

    protected override void OnKeyUp(KeyEventArgs e){
        if(e.KeyCode == Keys.Left ) mKeyL = 0;
        if(e.KeyCode == Keys.Right) mKeyR = 0;
        if(e.KeyCode == Keys.D    ) mKeyD = 0;
        if(e.KeyCode == Keys.A    ) mKeyA = 0;

        if(!mGameOver && e.KeyCode == Keys.Space) screen = 1;
        if(mGameOver && e.KeyCode == Keys.Escape) screen = 2;
    }

    protected override void OnLoad(EventArgs e){
        ClientSize = new Size(WND_WIDTH, WND_HEIGHT);
        Location = new Point(100, 50);
        DoubleBuffered = true;

        Bitmap bm = new Bitmap("tile.png");
        int BM_TILE = 18;
        int len = bm.Width / BM_TILE;//サイズ取得できるっぽい。18 = 切り抜き用サイズ
        mTile = new Bitmap[len];
        for(int i = 0; i < len; i++){
            mTile[i] = bm.Clone(new Rectangle(i * BM_TILE, 0, BM_TILE, BM_TILE), bm.PixelFormat);
        }//タイルの生成。切り抜きを配列に追加

        mNext = (byte)mRnd.Next(7);
        next();

        Task.Run(() =>{
            while( true ) {
                switch(screen){
                    case 0:
                        Invalidate();
                    break;
                    case 1:
                        onTimer();
                    break;
                    case 2:
                        mGameOver = false;
                        for(int y = 0; y < 25; y++){
                            for(int x = 0; x < MAP_WIDTH; x++){
                                mField[y, x] = InitField[y, x];
                            }
                        }//参照に注意。これしないと参照しておかしくなる。
                        mNext = (byte)mRnd.Next(7);
                        next();
                        screen = 0;
                        Invalidate();
                    break;
                }
                Task.Delay(TIMER_INTERVAL).Wait();
            }
        });
        
    }
    protected override void OnPaint(PaintEventArgs e){
        Graphics g = Graphics.FromImage(mScreen);//mScreenにgraphicsを書くようにする。実際の処理はそんな感じではないと思うけど。
        for(int y = 0; y < 25; y++){
            for(int x = 0; x < MAP_WIDTH; x++){
                g.DrawImage(mTile[mField[y, x]], x * TILE_SIZE, y * TILE_SIZE);
            }//次はmFieldを更新していって描画していくスタイル。
        }
        if(screen == 0){
            g.DrawString("Press Space", new Font("Meiryo", 16), Brushes.White, 16, 64);
        }

        if(mGameOver){
            g.DrawString("GameOver", new Font("Meiryo", 16), Brushes.White, 16, 64);
        }
        
        e.Graphics.DrawImage(mScreen, 0, 0, WND_WIDTH, WND_HEIGHT);//これの第三引数と第四引数は、指定されたものをそのサイズで描画するというやつ、つまり拡大されたり、縮小されたりする。
    }
    public static void Main(String[] args){
        Application.Run(new Tetorisu());
    }

    void next(){
        mX = 5;
        mY = 2;
        mT = mNext;
        mWait = WAIT;
        mA = 0;
        if(mKeyD > 0) mA = 3;
        if(mKeyA > 0) mA = 1;
        if(!put(mX, mY, mT, mA, true, false)){
            mGameOver = true;
        }

        put(5, -1, mNext, 0, false, false);//mNextに注意。入ってるものが違う。
        mNext = (byte)mRnd.Next(7);
        put(5, -1, mNext, 0, true, false);
    }

    void onTimer(){//ontimerはプログラム起動中はずっと実行されている。
        tick();
        if(mKeyL > 0) mKeyL++;
        if(mKeyR > 0) mKeyR++;
        if(mKeyA > 0) mKeyA++;
        if(mKeyD > 0) mKeyD++;
        mA &= 3;
        Invalidate();
    }

    public bool put(int x, int y, byte t, int a, bool vis, bool test){
        for (int j = 0; j < 4; j++){
            for (int i = 0; i < 4; i++){
                int[] p = {i, 3-j, 3-i,   j};
                int[] q = {j,   i, 3-j, 3-i};
                if(mBlock[t, q[a], p[a]] == 0){
                    continue;//次の処理をその回だけスキップ。
                    //ゼロのときは更新しない。
                }

                byte v = t;
                if(!vis){
                    v = 7;
                }else if(mField[y + j, x + i] != 7){
                    return false;
                }
                if(!test){//test（実験）をするかしないかのbool
                    mField[y + j, x + i] = v;//mFiledを更新する。
                    //mFieldの配列の(x,　y)を変える
                }
            }
        }
        return true;
    }

    void tick(){
        if(mGameOver){
            return;
        }
        if(mWait <= WAIT / 2){
            wait();
            return;
        }
        put(mX, mY, mT, mA, false, false);//putの第四引数のfalseは用は描画を消すやつ（7番にするやつ）
        //putはあくまでmField配列を書き換えるもの。だが第四引数をfalseにすることによって、そこに描画されてた形をあたかも消している科のようにしている。
        int a = mA;
        if(mKeyA == 1) a--;
        if(mKeyD == 1) a++;
        a &= 3;

        if(put(mX, mY, mT, a, true, true)){
            mA = a;
        }

        int ax = mX;
        if(mKeyL == 1 || mKeyL > 20) ax--;
        if(mKeyR == 1 || mKeyR > 20) ax++;
        if(put(ax, mY, mT, mA, true, true)){
            mX = ax;
        }
        if(put(mX, mY + 1, mT, mA, true, true)){
            mY++;
            mWait = WAIT;
        }else{
            mWait--;
        }
           
        put(mX, mY, mT, mA, true, false);
        //実は移動させているように見せかけているだけ。動かしたいブロックはmTで保持してる。
    }

    void wait(){
        if( --mWait == 0){
            next();
        }
        if(mWait == WAIT / 2 - 1){
            for (int y = 22; y > 2; y--){
                int n = 0;
                for ( int x = 2; x < 12; x++){
                    if(mField[y, x] < 7){
                        n++;
                    }
                }
                if(n != 10){
                    continue;
                }
                for(int x = 2; x < 12; x++){
                    mField[y, x] = 10;
                }
            }
        }
        if( mWait == 1){
            for( int y = 22; y > 2; y--){
                if(mField[y, 2] != 10){
                    continue;
                }
                mWait = WAIT / 2 -2;
                for( int i = y; i > 3; i--){
                    for( int x = 2; x < 12; x++){
                        mField[i, x] = mField[i - 1, x];
                    }
                }
                for( int x = 2; x < 12; x++){
                    mField[3, x] = 7;
                }
                y++;//一段下に下がるからその下がった分が処理されないのを防ぐ
            }
        }
    }
}