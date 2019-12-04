使用前请先阅读本文档：
1.图片处理部分目前支持烧录NAND(硬件的掉电存储片上结构)，和ddr烧录(内存）
2.同步数据至少必须大于代码中最大的register的值
3.由于编程的编程使用的是Scintilla(一个"大神"写的winform的框架的比较专业的文本编辑器)，
如果出现控件使用效果与预期不符，请设置一下兼容性。使用文档请查看github上的开源代码的wiki
-》https://github.com/jacobslusser/ScintillaNET/wiki
4.本程序中使用了mvvmlight，并且全局化了该框架，具体请查看App.xaml和App.xaml.cs文件。
5.cmd模式的烧录未完成，目前该部分已缺少图片发送程序。