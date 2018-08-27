﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Senparc.Weixin.MP.Containers;

namespace ToolKit
{
    public class WeixinApi
    {
		private const string WEIXIN_APPID_FIRST = "wx876a9766c264d457";
		private const string WEIXIN_APPSECRET_FIRST = "8155cb03c30692041526861ba2f08d1b";
		public string GetAccessToken(bool refresh=false)
		{
			var appId = WEIXIN_APPID_FIRST;
			var appSecret = WEIXIN_APPSECRET_FIRST;
			var cachekey = "weixinToken" + appId;
			var cache = System.Web.HttpRuntime.Cache.Get(cachekey);
			
			string accessToken = (string)cache;
			if (refresh) accessToken = "";
			if (string.IsNullOrEmpty(accessToken))
			{
				//AccessTokenContainer.Register(appId, appSecret);
				accessToken = AccessTokenContainer.TryGetAccessToken(appId,appSecret);
				System.Web.HttpRuntime.Cache.Insert(cachekey, accessToken, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromHours(1));
			}
			return accessToken;
		}

    }
}