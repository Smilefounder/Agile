using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UME_Music.Models
{
    public class T_playerConfig
    {
        /// <summary>
        /// 播放模式
        /// </summary>
        public int? PlayMode { get; set; }

        /// <summary>
        /// 音量
        /// </summary>
        public int? Volume { get; set; }

        /// <summary>
        /// 自动停止时间设定（秒）
        /// </summary>
        public int? AutoStopTimeSet { get; set; }
    }

    /// <summary>
    /// 播放模式
    /// </summary>
    public enum PlayModeEnum
    {
        /// <summary>
        /// 顺序播放
        /// </summary>
        Order = 0,

        /// <summary>
        /// 随机播放
        /// </summary>
        Random = 1,

        /// <summary>
        /// 列表循环
        /// </summary>
        Recycle = 2,

        /// <summary>
        /// 单曲循环
        /// </summary>
        Repeat = 3
    }

    /// <summary>
    /// 播放器状态
    /// </summary>
    public enum PlayerStateEnum
    {
        /// <summary>
        /// 就绪
        /// </summary>
        Ready = 0,

        /// <summary>
        /// 播放中
        /// </summary>
        Playing = 1,

        /// <summary>
        /// 已暂停
        /// </summary>
        Paused = 2
    }
}
