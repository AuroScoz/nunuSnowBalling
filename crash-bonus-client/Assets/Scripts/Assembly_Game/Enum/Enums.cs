using System;

namespace CrashBonus.Main {
    public enum Target {
        None,
        Myself,
        Enemy,
    }
    public enum Currency {
        Gold,
        Point,
    }
    public enum SaleState {
        OnSale,//上架中
        OffSale,//下架中
        ForTest,//測試商品，不會出現在Release版
    }
    public enum BuyLimitType {
        None,//無限定
        Permanence,//永久
        Daily,//每日
    }

    /// <summary>
    /// DB上的Collection
    /// </summary>
    public enum ColEnum {
        GameSetting,//遊戲設定
        Player,//玩家資料
        Item,//玩家道具
        History,//玩家紀錄
        Shop,//商城
        Purchase,//儲值商城
    }

    /// <summary>
    /// 顯示Google Ad廣告成功或失敗的回傳Enum訊息
    /// </summary>
    public enum AdsResultMessage {
        /// <summary>
        /// GoogleAd尚未初始化        
        /// </summary>
        GoogleAds_Not_Initialize,
        /// <summary>
        /// UnityAd尚未初始化        
        /// </summary>
        UnityAds_Not_Initialize,
        /// <summary>
        /// FacebookAd尚未初始化        
        /// </summary>
        FacebookAds_Not_Initialize,
        /// <summary>
        /// AppodealAd尚未初始化
        /// </summary>
        StartIoAds_Not_Initialize,
        /// <summary>
        /// GoogleAd已經有影片播放中
        /// </summary>
        Ads_AlreadyShowing,
        /// <summary>
        /// GoogleAds載入廣告失敗
        /// </summary>
        GoogleAdLoad_Fail,
        /// <summary>
        /// Unity載入廣告失敗
        /// </summary>
        UnityAdLoad_Fail,
        /// <summary>
        /// FacebookAd載入廣告失敗
        /// </summary>
        FacebookAdLoad_Fail,
        /// <summary>
        /// StartIoAd載入廣告失敗
        /// </summary>
        StartIoAdLoad_Fail,
        /// <summary>
        /// Google顯示廣告失敗
        /// </summary>
        GoogleAdShow_Fail,
        /// <summary>
        /// Unity顯示廣告失敗
        /// </summary>
        UnityAdShow_Fail,
        /// <summary>
        /// StartIo顯示廣告失敗
        /// </summary>
        StartIoAdShow_Fail,
        /// <summary>
        /// Google觀看廣告成功
        /// </summary>
        GoogleAdsWatchSuccess,
        /// <summary>
        /// Unity觀看廣告成功
        /// </summary>
        UnityAdsWatchSuccess,
        /// <summary>
        /// Facebook觀看廣告成功
        /// </summary>
        FacebookAdsWatchSuccess,
        /// <summary>
        /// AStartIo觀看廣告成功
        /// </summary>
        StartIoAdsWatchSuccess,
        /// <summary>
        /// 不觀看廣告直接給獎成功
        /// </summary>
        DontShowAdSuccess,
        /// <summary>
        /// 不觀看廣告直接給獎失敗
        /// </summary>
        DontShowAdFail,
        /// <summary>
        /// GoogleAds廣告還沒準備好
        /// </summary>
        GoogleAds_NotReady,
        /// <summary>
        /// UnityAds廣告還沒準備好
        /// </summary>
        UnityAds_NotReady,
        /// <summary>
        /// FacebookAds廣告還沒準備好
        /// </summary>
        FacebookAds_NotReady,
        /// <summary>
        /// StartIoAds廣告還沒準備好
        /// </summary>
        StartIoAds_NotReady,
    }
    public enum BattleSkillType {
        /// <summary>
        /// 直接觸發
        /// </summary>
        Instant,
        /// <summary>
        /// 碰撞觸發
        /// </summary>
        Melee,
    }
}