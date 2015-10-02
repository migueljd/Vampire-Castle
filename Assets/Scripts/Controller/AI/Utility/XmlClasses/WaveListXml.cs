using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot("WaveList")]
public class WaveListXml {

	[XmlArray("Waves")]
	[XmlArrayItem("Wave")]
	public List<WaveXml> waves = new List<WaveXml>();

}
