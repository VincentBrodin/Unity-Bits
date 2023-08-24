using Steamworks;
using UnityEngine;

public static class SteamBits
{
    public static Texture2D GetSmallAvatar(CSteamID user)
    {
        var friendAvatar = SteamFriends.GetSmallFriendAvatar(user);
        var success = SteamUtils.GetImageSize(friendAvatar, pnWidth: out var imageWidth, out var imageHeight);

        if (success && imageWidth > 0 && imageHeight > 0)
        {
            var image = new byte[imageWidth * imageHeight * 4];
            var returnTexture = new Texture2D((int)imageWidth, (int)imageHeight, TextureFormat.RGBA32, false, true);
            success = SteamUtils.GetImageRGBA(friendAvatar, image, (int)(imageWidth * imageHeight * 4));
            if (!success) return returnTexture;
            returnTexture.LoadRawTextureData(image);
            returnTexture.Apply();
            return returnTexture;
        }
        else
        {
            Debug.LogError("Couldn't get avatar.");
            return new Texture2D(0, 0);
        }
    }
    
    public static Texture2D GetMediumAvatar(CSteamID user)
    {
        var friendAvatar = SteamFriends.GetMediumFriendAvatar(user);
        var success = SteamUtils.GetImageSize(friendAvatar, pnWidth: out var imageWidth, out var imageHeight);

        if (success && imageWidth > 0 && imageHeight > 0)
        {
            var image = new byte[imageWidth * imageHeight * 4];
            var returnTexture = new Texture2D((int)imageWidth, (int)imageHeight, TextureFormat.RGBA32, false, true);
            success = SteamUtils.GetImageRGBA(friendAvatar, image, (int)(imageWidth * imageHeight * 4));
            if (!success) return returnTexture;
            returnTexture.LoadRawTextureData(image);
            returnTexture.Apply();
            return returnTexture;
        }
        else
        {
            Debug.LogError("Couldn't get avatar.");
            return new Texture2D(0, 0);
        }
    }
    
    public static Texture2D GetLargeAvatar(CSteamID user)
    {
        var friendAvatar = SteamFriends.GetLargeFriendAvatar(user);
        var success = SteamUtils.GetImageSize(friendAvatar, pnWidth: out var imageWidth, out var imageHeight);

        if (success && imageWidth > 0 && imageHeight > 0)
        {
            var image = new byte[imageWidth * imageHeight * 4];
            var returnTexture = new Texture2D((int)imageWidth, (int)imageHeight, TextureFormat.RGBA32, false, true);
            success = SteamUtils.GetImageRGBA(friendAvatar, image, (int)(imageWidth * imageHeight * 4));
            if (!success) return returnTexture;
            returnTexture.LoadRawTextureData(image);
            returnTexture.Apply();
            return returnTexture;
        }
        else
        {
            Debug.LogError("Couldn't get avatar.");
            return new Texture2D(0, 0);
        }
    }
}
