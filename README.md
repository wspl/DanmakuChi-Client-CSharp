# DanmakuChi-Client-CSharp
弹幕姬客户端 .Net(C#) 版本

## 介绍
　　弹幕姬是一款「微信x桌面」弹幕系统，开发者可以通过轻松的绑定，在微信公众号与桌面弹幕之间建立交互关系。任何人都可以通过微信公众号创建自己专属的弹幕频道，任何用户都可以通过微信向客户端发送弹幕。

　　其中，服务器端（Server End）负责微信的交互，以及微信与客户端的中转对接。通常的，服务端需要一台开放 80 端口的公网服务器来运行。目前已经开发了 [Node 版本](https://github.com/wspl/DanmakuChi-Server-Node)。

　　客户端（Client End）负责与服务端的交互，进行弹幕的展示。目前已经开发了 [C# 版本](https://github.com/wspl/DanmakuChi-Client-CSharp)。


## 技术栈
该 C# 版本的客户端中，涉及到的技术栈如下：
* Visual Studio Community 2015
* .Net Framework 4.0
* WPF (Windows Presentation Foundation)
* WebSocket

## TODO List
- [ ] 使用 GDI+ 渲染弹幕（待定）
- [ ] 增加弹幕颜色设置支持
- [ ] 增加弹幕位置设置支持
- [x] 更科学的弹幕飞行速度

## Contributor
* [Plutonist(wspl)](https://github.com/wspl)

## License
GNU GPL V3