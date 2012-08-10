/* Copyright (c) 2012 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.IO;
using Gibbed.Avalanche.RenderBlockModel;
using SlimDX.Direct3D10;

namespace Gibbed.Avalanche.ModelViewer2
{
    internal class MaterialLoader : IDisposable
    {
        private Texture2D _TextureUndamagedDiffuseTexture;
        private Texture2D _TextureUndamagedNormalMap;
        private Texture2D _TextureUndamagedPropertiesMap;
        private Texture2D _TextureDamagedDiffuseTexture;
        private Texture2D _TextureDamagedNormalMap;
        private Texture2D _TextureDamagedPropertiesMap;

        private ShaderResourceView _ResourceUndamagedDiffuseTexture;
        private ShaderResourceView _ResourceUndamagedNormalMap;
        private ShaderResourceView _ResourceUndamagedPropertiesMap;
        private ShaderResourceView _ResourceDamagedDiffuseTexture;
        private ShaderResourceView _ResourceDamagedNormalMap;
        private ShaderResourceView _ResourceDamagedPropertiesMap;

        private static Texture2D LoadTextureFrom(Device device, string basePath, string fileName)
        {
            if (string.IsNullOrEmpty(fileName) == true)
            {
                return null;
            }

            string filePath = Path.Combine(basePath, fileName);
            if (File.Exists(filePath) == false)
            {
                return null;
            }

            return Texture2D.FromFile(device, filePath);
        }

        public void Setup(Device device, string basePath, Material material)
        {
            this._TextureUndamagedDiffuseTexture = LoadTextureFrom(device, basePath, material.UndeformedDiffuseTexture);
            this._TextureUndamagedNormalMap = LoadTextureFrom(device, basePath, material.UndeformedNormalMap);
            this._TextureUndamagedPropertiesMap = LoadTextureFrom(device, basePath, material.UndeformedPropertiesMap);
            
            this._TextureDamagedDiffuseTexture = LoadTextureFrom(device, basePath, material.DeformedDiffuseTexture);
            this._TextureDamagedNormalMap = LoadTextureFrom(device, basePath, material.DeformedNormalMap);
            this._TextureDamagedPropertiesMap = LoadTextureFrom(device, basePath, material.DeformedPropertiesMap);

            if (this._TextureUndamagedDiffuseTexture != null)
            {
                this._ResourceUndamagedDiffuseTexture = new ShaderResourceView(device, this._TextureUndamagedDiffuseTexture);
            }

            if (this._TextureUndamagedNormalMap != null)
            {
                this._ResourceUndamagedNormalMap = new ShaderResourceView(device, this._TextureUndamagedNormalMap);
            }

            if (this._TextureUndamagedPropertiesMap != null)
            {
                this._ResourceUndamagedPropertiesMap = new ShaderResourceView(device, this._TextureUndamagedPropertiesMap);
            }

            if (this._TextureDamagedDiffuseTexture != null)
            {
                this._ResourceDamagedDiffuseTexture = new ShaderResourceView(device, this._TextureDamagedDiffuseTexture);
            }

            if (this._TextureDamagedNormalMap != null)
            {
                this._ResourceDamagedNormalMap = new ShaderResourceView(device, this._TextureDamagedNormalMap);
            }

            if (this._TextureDamagedPropertiesMap != null)
            {
                this._ResourceDamagedPropertiesMap = new ShaderResourceView(device, this._TextureDamagedPropertiesMap);
            }
        }

        public void SetShaderResource(Device device)
        {
            device.PixelShader.SetShaderResource(this._ResourceUndamagedDiffuseTexture, 0);
            device.PixelShader.SetShaderResource(this._ResourceUndamagedNormalMap, 1);
            device.PixelShader.SetShaderResource(this._ResourceUndamagedPropertiesMap, 2);
            device.PixelShader.SetShaderResource(this._ResourceDamagedDiffuseTexture, 3);
            device.PixelShader.SetShaderResource(this._ResourceDamagedNormalMap, 4);
            device.PixelShader.SetShaderResource(this._ResourceDamagedPropertiesMap, 5);
        }

        public void Dispose()
        {
            if (this._ResourceUndamagedPropertiesMap != null)
            {
                this._ResourceUndamagedPropertiesMap.Dispose();
            }

            if (this._ResourceUndamagedNormalMap != null)
            {
                this._ResourceUndamagedNormalMap.Dispose();
            }

            if (this._ResourceUndamagedDiffuseTexture != null)
            {
                this._ResourceUndamagedDiffuseTexture.Dispose();
            }

            if (this._TextureUndamagedPropertiesMap != null)
            {
                this._TextureUndamagedPropertiesMap.Dispose();
            }

            if (this._TextureUndamagedNormalMap != null)
            {
                this._TextureUndamagedNormalMap.Dispose();
            }

            if (this._TextureUndamagedDiffuseTexture != null)
            {
                this._TextureUndamagedDiffuseTexture.Dispose();
            }

        }
    }
}
