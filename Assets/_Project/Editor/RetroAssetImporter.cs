using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Frustration.EditorTools
{
    // Всё, что кладётся в Assets/_ThirdParty/ или Assets/_Project/Models/, сразу приводится к PS1-стилю:
    // - текстуры: без сглаживания (Point), без mip-карт, не больше 256 px;
    // - модели: без камер, света и анимаций;
    // - материалы моделей: наш шейдер "Frustration/PS1 Lit" с отсечением прозрачных пикселей
    //   и двусторонней отрисовкой (иначе листва деревьев видна только с одной стороны).
    class RetroAssetImporter : AssetPostprocessor
    {
        static bool InScope(string path) =>
            path.StartsWith("Assets/_ThirdParty/") || path.StartsWith("Assets/_Project/Models/");

        // Запускаемся после встроенной обработки URP, чтобы наш шейдер был последним словом.
        public override int GetPostprocessOrder() => 100;

        void OnPreprocessTexture()
        {
            if (!InScope(assetPath)) return;
            var importer = (TextureImporter)assetImporter;
            importer.filterMode = FilterMode.Point;
            importer.mipmapEnabled = false;
            importer.maxTextureSize = 256;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = importer.DoesSourceTextureHaveAlpha();
        }

        void OnPreprocessModel()
        {
            if (!InScope(assetPath)) return;
            var importer = (ModelImporter)assetImporter;
            importer.importCameras = false;
            importer.importLights = false;
            importer.importAnimation = false;
            importer.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
        }

        void OnPreprocessMaterialDescription(MaterialDescription description, Material material, AnimationClip[] animations)
        {
            if (!InScope(assetPath)) return;
            var shader = Shader.Find("Frustration/PS1 Lit");
            if (shader == null) return;

            Texture texture = null;
            if (description.TryGetProperty("DiffuseColor", out TexturePropertyDescription diffuse)) texture = diffuse.texture;
            else if (description.TryGetProperty("BaseColor", out TexturePropertyDescription baseColor)) texture = baseColor.texture;

            Color color = Color.white;
            if (texture == null && description.TryGetProperty("DiffuseColor", out Vector4 diffuseColor))
                color = new Color(diffuseColor.x, diffuseColor.y, diffuseColor.z, 1f);

            material.shader = shader;
            material.SetTexture("_BaseMap", texture);
            material.SetColor("_BaseColor", color);
            material.SetColor("_EmissionColor", Color.black);
            material.SetFloat("_Cutoff", 0.5f);
            material.SetFloat("_Cull", 0f);
        }
    }
}
