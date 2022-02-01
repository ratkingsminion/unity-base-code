using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base {

	public static class MaterialExtras {

		public class Pool {
			static Dictionary<Material, Dictionary<Color, Material>> usedMaterialByColor = new Dictionary<Material, Dictionary<Color, Material>>();
			static Dictionary<Material, Dictionary<Shader, Material>> usedMaterialByShader = new Dictionary<Material, Dictionary<Shader, Material>>();
			static Dictionary<Material, Material> usedMaterialByShader_R = new Dictionary<Material, Material>();

			public static Material Get(Material material, Color color) {
				if (!usedMaterialByColor.ContainsKey(material))
					usedMaterialByColor[material] = new Dictionary<Color, Material>();

				if (!usedMaterialByColor[material].ContainsKey(color)) {
					Material newMaterial = (Material)Material.Instantiate(material);
					newMaterial.color = color;
					usedMaterialByColor[material][color] = newMaterial;
					return newMaterial;
				}

				return usedMaterialByColor[material][color];
			}
			
			public static Material Get(Material material, Shader shader) {
				if (!usedMaterialByShader.ContainsKey(material)) {
					usedMaterialByShader[material] = new Dictionary<Shader, Material>();
				}
				if (!usedMaterialByShader[material].ContainsKey(shader)) {
					Material newMaterial = (Material)Material.Instantiate(material);
					newMaterial.shader = shader;
					usedMaterialByShader[material][shader] = newMaterial;
					usedMaterialByShader_R[newMaterial] = material;
					return newMaterial;
				}
				return usedMaterialByShader[material][shader];
			}
			public static Material Reset(Material manipulatedMaterial) {
				if (!usedMaterialByShader_R.ContainsKey(manipulatedMaterial)) {
					return null;
				}
				return usedMaterialByShader_R[manipulatedMaterial];
			}
		}

	}
	
}