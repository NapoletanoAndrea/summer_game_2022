using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ElementType {
	Rock,
	Paper,
	Scissors
}

public static class Elements
{
	public static int Compare(this ElementType element1, ElementType element2) {
		switch (element1) {
			case ElementType.Rock:
				switch (element2) {
					case ElementType.Rock: return 0;
					case ElementType.Paper: return -1;
					case ElementType.Scissors: return 1;
				}
				break;
			case ElementType.Paper:
				switch (element2) {
					case ElementType.Rock: return 1;
					case ElementType.Paper: return 0;
					case ElementType.Scissors: return -1;
				}
				break;
			case ElementType.Scissors:
				switch (element2) {
					case ElementType.Rock: return -1;
					case ElementType.Paper: return 1;
					case ElementType.Scissors: return 0;
				}
				break;
		}
		return 0;
	}
}
