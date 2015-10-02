using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.Collections.Generic;


[XmlRoot("Wave")]
public class WaveXml{
	[XmlArray("EnemiesList")]
	[XmlArrayItem("Enemy")]
	public List<string> enemiesName = new List<string>();

	[XmlElement("TimerPerEnemy")]
	public float timer;
}
