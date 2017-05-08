**Unity4AutoBuildTools(Android/IOS)**
=
Unity4自動產檔工具(Android/IOS)
=

Unity5用的版本還在調整

簡單說這個小工具只是方便我自動化產出Android/IOS的時候所寫的

- 主要有下面幾種功能
1. 修改Define與PlayerSetting>>>DefineSetting工具
2. 檔案搬移或刪除
3. Unity產出XCdoe專案之後的設定
4. 自動化整合以上功能
* * *
用於儲存客製化的DefineSetting是引用到了[Newtonsoft的Json.net](http://www.newtonsoft.com/json) 
有些設定在Unity提供的Playersetting方法找不到，
所以從網路上搜尋直接把ProjectSettings.asset的檔案讀出來
再複寫回去的方法
如果有更好的寫法會再去修改的
* * *
而XCodeEditor因為Unity4沒有支援所以參照了[官方](https://bitbucket.org/Unity-Technologies/xcodeapi)的API功能
XCodeEditor的做法主要參考[NORTHERN WIND(部落格)](https://blog.chunfuchao.com/?p=359&variant=zh-tw)
Framework的tbd檔案追加是參考[qiita.com的答案](http://qiita.com/tkyaji/items/19dfff4afe228c7f34a1)
* * * 
或許這些功能很平常
但是對於我在服務的一些專案做到自動化的幫助還挺大的
Unity5的版本還在修正中
整理好也會放上來分享的

