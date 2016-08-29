using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RatKing.Base.Helpers {

	public static class Materials {

		public class Pool {
			private static Dictionary<Material, Dictionary<Color, Material>> usedMaterials = new Dictionary<Material, Dictionary<Color, Material>>();
			public static void Add(Material material) {
				if (!usedMaterials.ContainsKey(material))
					usedMaterials[material] = new Dictionary<Color, Material>();
				usedMaterials[material][material.color] = material;
			}
			public static Material Get(Material material, Color color) {
				if (!usedMaterials.ContainsKey(material))
					usedMaterials[material] = new Dictionary<Color, Material>();

				if (!usedMaterials[material].ContainsKey(color)) {
					Material newMaterial = (Material)Material.Instantiate(material);
					newMaterial.color = color;
					usedMaterials[material][color] = newMaterial;
					return newMaterial;
				}

				return usedMaterials[material][color];
			}
		}

	}
	
}