using Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Platform
{
    public static class Tool
    {
        public static string NO_PROFILE_URL = "NO_PROFILE";
        public static readonly decimal MB = 1_048_576;
        public static readonly string RgxEmail = @"(^\w{2,})(@(\w+)(\.(\w+))+$)";

        public static Sprite CreateSprite(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }

        public static string AddUrlProtocolIfDontHave(string avatarUrl)
        {
            if (string.IsNullOrEmpty(avatarUrl))
            {
                return string.Empty;
            }

            if (avatarUrl.Contains(NO_PROFILE_URL))
            {
                return string.Empty;
            }

            if (avatarUrl.StartsWith("//"))
            {
#if !IS_QASTATION
                return $"http:{avatarUrl}";
#else
                return $"https:{avatarUrl}";
#endif
            }
            return avatarUrl;
        }

        public static bool IsAccountFieldValid(this char c)
        {
            if (c >= 'A' && c <= 'Z')
            {
                return true;
            }
            if (c >= 'a' && c <= 'z')
            {
                return true;
            }
            if (c >= '0' && c <= '9')
            {
                return true;
            }
            if (c == '@' || c == '.')
            {
                return true;
            }
            return false;
        }

        private static void DownLoadImage(string url, Action<Sprite> onLoaded)
        {
            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);
            var op = uwr.SendWebRequest();
            op.completed += OnCompleted;

            void OnCompleted(AsyncOperation asyncOperation)
            {
                op.completed -= OnCompleted;
                var data = uwr.downloadHandler.data;
                onLoaded?.Invoke(CreateSpriteFromBytes(data));
                uwr.Dispose();
            }
        }
        public static Sprite CreateSpriteFromBytes(byte[] bytes)
        {
            if (CreateTextureFromBytes(bytes, out var texture))
            {
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
            return null;
        }

        public static bool CreateTextureFromBytes(byte[] bytes, out Texture2D texture)
        {
            texture = new Texture2D(2, 2);
            var isLoad = texture.LoadImage(bytes);
            if (!isLoad)
            {
                Debug.LogError($"載入Texture失敗 : {JsonConvert.SerializeObject(bytes)}");
                return false;
            }
            return true;
        }


        /// <summary>
        /// 运行模式下Texture转换成Texture2D
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static Texture2D TextureToTexture2D(Texture texture)
        {
            Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
            Graphics.Blit(texture, renderTexture);

            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(renderTexture);

            return texture2D;
        }
    }
}