using System;
using UnityEngine;

namespace SecondSpring.IntegratedPrototype
{
    public sealed class IntegratedPrototypeArt : ScriptableObject
    {
        [Header("Giao diện và cây trồng")]
        public Texture2D calendar, hole, moleArt;
        public Texture2D soilAndCropAtlas, seedAndProduceAtlas, coin;
        public Texture2D[] toolIcons;
        public Texture2D waterDrop, crate;

        [Header("Vật nuôi")]
        public Texture2D[] penTextures, adultTextures, youngTextures, feedTextures, fishTextures;

        [Header("Phong cảnh")]
        public Sprite windmillBase, windmillSails, cloud, smoke;
        public Texture2D dayHorizon, nightHorizon;

        [Header("Âm thanh")]
        public AudioClip[] musicTracks = Array.Empty<AudioClip>();
        public AudioClip clickSound;
        public bool enableClickSounds;
        [Range(0, 1)] public float musicVolume = .25f, effectVolume = .45f;
    }
}
