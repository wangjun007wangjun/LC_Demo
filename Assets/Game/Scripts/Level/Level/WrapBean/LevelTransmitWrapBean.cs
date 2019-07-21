using UnityEngine;

public class LevelTransmitWrapBean : ALevelItemViewWrapBean
 {
     public int transmitId { get; private set; }
 
     public LevelTransmitWrapBean(LevelItemType type,
                                  GameObject prefab,
                                  int index,
                                  float size,
                                  int transmitId) : base(type, prefab, index, size)
     {
         this.transmitId = transmitId;
     }
 }