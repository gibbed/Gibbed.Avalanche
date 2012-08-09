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
        private Texture2D _TextureDiffuseTexture;
        private Texture2D _TextureNormalMap;
        private Texture2D _TexturePropertiesMap;

        private ShaderResourceView _ResourceDiffuseTexture;
        private ShaderResourceView _ResourceNormalMap;
        private ShaderResourceView _ResourcePropertiesMap;

        private static Texture2D LoadTextureFrom(Device device, string basePath, string fileName)
        {
            string filePath = Path.Combine(basePath, fileName);

            if (string.IsNullOrEmpty(fileName) == true ||
                File.Exists(filePath) == false)
            {
                return null;
            }

            return Texture2D.FromFile(device, filePath);
        }

        public void Setup(Device device, string basePath, Material material)
        {
            this._TextureDiffuseTexture = LoadTextureFrom(device, basePath, material.DiffuseTexture);
            this._TextureNormalMap = LoadTextureFrom(device, basePath, material.NormalMap);
            this._TexturePropertiesMap = LoadTextureFrom(device, basePath, material.PropertiesMap);

            if (this._TextureDiffuseTexture != null)
            {
                this._ResourceDiffuseTexture = new ShaderResourceView(device, this._TextureDiffuseTexture);
            }

            if (this._TextureNormalMap != null)
            {
                this._ResourceNormalMap = new ShaderResourceView(device, this._TextureNormalMap);
            }

            if (this._TexturePropertiesMap != null)
            {
                this._ResourcePropertiesMap = new ShaderResourceView(device, this._TexturePropertiesMap);
            }
        }

        public void SetShaderResource(Device device)
        {
            device.PixelShader.SetShaderResource(this._ResourceDiffuseTexture, 0);
            device.PixelShader.SetShaderResource(this._ResourceNormalMap, 1);
            device.PixelShader.SetShaderResource(this._ResourcePropertiesMap, 2);
            /*device.PixelShader.SetShaderResource(this._ResourceDiffuseTexture, 3);
            device.PixelShader.SetShaderResource(this._ResourceNormalMap, 4);
            device.PixelShader.SetShaderResource(this._ResourcePropertiesMap, 5);
            device.PixelShader.SetShaderResource(this._ResourceDiffuseTexture, 6);
            device.PixelShader.SetShaderResource(this._ResourceNormalMap, 7);
            device.PixelShader.SetShaderResource(this._ResourcePropertiesMap, 8);*/
        }

        public void Dispose()
        {
            if (this._ResourcePropertiesMap != null)
            {
                this._ResourcePropertiesMap.Dispose();
            }

            if (this._ResourceNormalMap != null)
            {
                this._ResourceNormalMap.Dispose();
            }

            if (this._ResourceDiffuseTexture != null)
            {
                this._ResourceDiffuseTexture.Dispose();
            }

            if (this._TexturePropertiesMap != null)
            {
                this._TexturePropertiesMap.Dispose();
            }

            if (this._TextureNormalMap != null)
            {
                this._TextureNormalMap.Dispose();
            }

            if (this._TextureDiffuseTexture != null)
            {
                this._TextureDiffuseTexture.Dispose();
            }

        }
    }
}
