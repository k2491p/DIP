# SOLIDの原則とは？

SOLIDは

- 変更に強い
- 理解しやすい
などのソフトウェアを作ることを目的とした原則です。

次の5つの原則があります。

- Single Responsibility Principle (単一責任の原則)
- Open-Closed Principle (オープン・クローズドの原則)
- Liskov Substitution Principle (リスコフの置換原則)
- Interface Segregation Principle (インタフェース分離の原則)
- Dependency Inversion Principle (依存関係逆転の原則)

上記5つの原則の頭文字をとって**SOLID**の原則と言います。
今回の記事では **Dependency Inversion Principle (依存関係逆転の原則)** について解説します。
その他の原則に関しては下記参照。
※随時追加予定！

# 簡単に言うと...

「**使う側と使われる側の関係を見直そう**」ということです。
たとえば、画面で保存ボタンを押したらDBに保存されるというシンプルな機能について考えましょう。

※ここに画像

この場合、

- 「画面」が使う側
- 「DBに保存する処理」が使われる側

になります。
普通処理を書こうとすると、「画面」から「DBに保存する処理」を呼び出すため、
「画面」が「DBに保存する処理」に依存することになります。
たとえば、保存先がDBからCSVファイルに切り替わった場合、その影響が画面 (正確に言うと画面側での呼び出しロジック) にも影響が出てしまいます。
この影響を出ないようにするために、「**使う側と使われる側の関係を見直そう**」というのです。
では、使う側と使われる側の関係を見直した結果、どのようにすればよいのでしょうか？

# 少し詳しめに言うと...

Wikipediaでは下記のように説明されています。

> 1. 上位モジュールはいかなるものも下位モジュールから持ち込んではならない。双方とも抽象（例としてインターフェース）に依存するべきである。
>
> 2. 抽象は詳細に依存してはならない。詳細（具象的な実装内容）が抽象に依存するべきである。

先程の「画面」から「DBに保存する処理」に上記を適用すると下図の用になります。

※ここに画像

> 1. 上位モジュールはいかなるものも下位モジュールから持ち込んではならない。双方とも抽象（例としてインターフェース）に依存するべきである。

元々は使う側 (View) が使われる側 (Repository) に依存していましたが、
IRepositoryという抽象 (ここではインターフェース) が現れ、使う側 (View) も使われる側 (Repository) も抽象 (IRepository) に依存しています。

>2. 抽象は詳細に依存してはならない。詳細（具象的な実装内容）が抽象に依存するべきである。

IRepositoryにSaveというメソッドが定義されており、Repositoryはそれを継承して使っています。これは、詳細 (Repository) の具体的な実装内容が、抽象 (IRepository) に依存しているということです。

もう少し具体例を用いながら見ていきましょう。

# 今回使用する例

テレビの録画を例に考えてみましょう。

※ここに画像

テレビの録画機器として、ビデオ、ブルーレイディスク、HDDなどがあります。
これをコードに落とし込んでいきましょう。

# 依存関係逆転の原則に違反した例

ビデオ、ブルーレイディスク、HDDの3つのクラスを作り、
テレビから呼び出すという想定で考えてみましょう。

※ここに画像

```c#:Television.cs
class Television
{
	public void Record()
    {
        if (ConnectsVideo())
        {
            Video video = new Video();
			video.Record();
            return;
        }
        
        if (ConnectsBluRay())
        {
            Blu_ray bluRay = new Blu_ray();
            bluRay.Record();
            return;
        }
        
        HDD hdd = new HDD();
        hdd.Record();
    }
}
```

```c#:Video.cs
public sealed class Video
{
    public void Record()
    {
        // ビデオに録画
    }
}
```

```c#:Blu_ray.cs
public sealed class Blu_ray
{
    public void Record()
    {
        // ブルーレイに録画
    }
}
```

```c#:HDD.cs
public sealed class HDD
{
    public void Record()
    {
        // HDDに録画
    }
}
```

テレビ側で、逐一「記録媒体が何であるか」を確認して、各記録媒体に記録指示を出すことになります。
これが、使う側が使われる側に依存している状態です。



# 依存関係逆転の法則を適用する
テレビと記録媒体の間にインターフェースをかませることで解決できます。

※ここに画像

コードでの実装例は下記のようになります。

```c#:Television.cs
class Television
{
	public void Record()
    {
        IRecord recorder = Factories.CreateRecord();
        recorder.Record();
    }
}
```

```c#:IRecord.cs
public interface IRecord
{
    enum Recorder 
    {
        VIDEO,
        BLU_RAY,
        HDD
    }

    public void Record();
}
```

```c#:Video.cs
public sealed class Video : IRecord
{
    public void Record()
    {
        // ビデオに録画
    }
}
```

```c#:Blu_ray.cs
public sealed class Blu_ray : IRecord
{
    public void Record()
    {
        // ブルーレイに録画
    }
}
```

```c#:HDD.cs
public sealed class HDD : IRecord
{
    public void Record()
    {
        // HDDに録画
    }
}
```

```c#:Factories.cs
public static class Factories
{
    public static IRecord CreateRecord()
    {
        // テレビに接続されている機器取得
        var recorder = GetRecorder();

        if (recorder == IRecord.Recorder.VIDEO)
        {
            return new Video();
        }

        if (recorder == IRecord.Recorder.BLU_RAY)
        {
            return new Blu_ray();
        }

        return new HDD();
    }
}
```

Factoryを使うことで、テレビ側での呼び出しを簡素にしています。
これにより、ビデオなどの具象クラスになにかの変更があったとしても、その変更の影響が使う側には伝わりにくくなります。
また、それ以外のメリットもあります。

# テスト容易姓

依存関係逆転の法則を適用することは、テストをしやすくなることにも繋がります。
例えば、上記例では、

- ビデオ
- ブルーレイディスク
- HDD

などの具体的なものに繋いでの動作確認が必要になります。
もし、それらの機器が手元にない場合、テストをすることができません。
このように、外部機器との接続が必要なものの場合、テスト用のクラス (Fakeクラスなど) を用意しておくことで
テストがしやすくなります。

※ここに画像

例えば、Factoriesにこのように書いておくことで、デバッグ時は常にFakeから値を返すということができます。

```c#:Factories.cs
public static class Factories
{
    public static IRecord CreateRecord()
    {
        // ここを追加
#if DEBUG
        return new Fake();
#endif

        // テレビに接続されている機器取得
        var recorder = GetRecorder();

        if (recorder == IRecord.Recorder.VIDEO)
        {
            return new Video();
        }

        if (recorder == IRecord.Recorder.BLU_RAY)
        {
            return new Blu_ray();
        }

        return new HDD();
    }
}
```



# まとめ
依存関係逆転の法則とは「**使う側と使われる側の関係を見直そう**」ということでした。
具体的には、

- 使う側も使われる側も抽象に依存させる
- 詳細の処理内容を抽象に依存させる

ということです。
そうすることで下記のようなメリットが得られます。

- 変更に強い
- 理解しやすい

尚、今回使用したソースは[こちら](https://github.com/k2491p/DIP)に上がっています。



# 参考文献
- 本
  - [Clean Architecture 達人に学ぶソフトウェアの構造と設計](https://www.amazon.co.jp/Clean-Architecture-%E9%81%94%E4%BA%BA%E3%81%AB%E5%AD%A6%E3%81%B6%E3%82%BD%E3%83%95%E3%83%88%E3%82%A6%E3%82%A7%E3%82%A2%E3%81%AE%E6%A7%8B%E9%80%A0%E3%81%A8%E8%A8%AD%E8%A8%88-Robert-C-Martin/dp/4048930656)
- サイト
  - [1分でわかる依存関係逆転の原則(DIP)](https://qiita.com/wanko5296/items/29e74cc7dd7562624d08#dipdependency-inversion-principle---%E4%BE%9D%E5%AD%98%E9%96%A2%E4%BF%82%E9%80%86%E8%BB%A2%E3%81%AE%E5%8E%9F%E5%89%87)
  - [依存関係逆転の法則とクリーンアーキテクチャ](https://qiita.com/Koichi/items/e1ab6b3a66f65c8ee91c)
  - [よくわかるSOLID原則5: D（依存性逆転の原則）](https://note.com/erukiti/n/n913e571e8207)
  - [【SOLID原則】依存性逆転の原則 - DIP](https://zenn.dev/chida/articles/e46a66cd9d89d1)
  - [依存性逆転の原則 - Wikipedia](https://ja.wikipedia.org/wiki/%E4%BE%9D%E5%AD%98%E6%80%A7%E9%80%86%E8%BB%A2%E3%81%AE%E5%8E%9F%E5%89%87)
- 動画
  - [オブジェクト指向の原則３：依存関係逆転の原則とインタフェース分離の原則](https://www.udemy.com/course/objectfive3/)