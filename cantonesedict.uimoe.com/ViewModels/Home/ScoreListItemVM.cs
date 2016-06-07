using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cantonesedict.uimoe.com.ViewModels.Home
{
    public class ScoreListItemVM
    {
        public int score { get; set; }

        public int way { get; set; }

        public string waydisplay
        {
            get
            {
                switch (way)
                {
                    default:
                        return "其他途径获得";
                    case (int)ScoreListItemWayEnum.Query:
                        return "查询获得积分";
                    case (int)ScoreListItemWayEnum.Login:
                        return "登录获得积分";
                    case (int)ScoreListItemWayEnum.CantoneseTest:
                        return "粤语水平测试获得积分";
                    case (int)ScoreListItemWayEnum.TranslateLyric:
                        return "歌词翻译获得积分";
                }
            }
        }

        public DateTime? createdat { get; set; }
    }

    public enum ScoreListItemWayEnum
    {
        /// <summary>
        /// 查询获得积分
        /// </summary>
        Query = 0,

        /// <summary>
        /// 登录获得积分
        /// </summary>
        Login = 1,

        /// <summary>
        /// 粤语水平测试获得积分
        /// </summary>
        CantoneseTest = 2,

        /// <summary>
        /// 歌词翻译获得积分
        /// </summary>
        TranslateLyric = 3
    }
}