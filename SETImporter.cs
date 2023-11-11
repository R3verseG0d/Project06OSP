using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Xml;
using UnityEngine;

public static class SETImporter
{
	private static float Scale = 0.0105f;

	public static void CreateObjects(TextAsset SetFile, string ImportClass)
	{
		Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(SetFile.text);
		XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("objectplacement");
		if (elementsByTagName.Count == 0)
		{
			Debug.LogError("XML set file " + SetFile.name + " is invalid.");
			return;
		}
		Transform transform = GameObject.FindGameObjectsWithTag("StageManager")[0].FindInChildren("Objects").transform;
		Transform transform2 = transform.FindInChildren("Static").transform;
		Transform transform3 = transform.FindInChildren("Physics").transform;
		Transform transform4 = transform.FindInChildren("Enemies").transform;
		Transform transform5 = transform.FindInChildren("Effects").transform;
		Transform transform6 = transform.FindInChildren("Static_DHide").transform;
		foreach (XmlNode item in elementsByTagName)
		{
			XmlNodeList childNodes = item.ChildNodes;
			if (childNodes.Count == 0)
			{
				Debug.LogError("XML set file " + SetFile.name + " has invalid object list.");
				continue;
			}
			foreach (XmlNode item2 in childNodes)
			{
				string value = item2.Attributes["type"].Value;
				bool flag = true;
				string[] array = ImportClass.Split(","[0]);
				foreach (string text in array)
				{
					if (value == text)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
				GameObject gameObject = null;
				gameObject = item2.CreateObject(value, childNodes);
				if (gameObject == null)
				{
					continue;
				}
				gameObject.name = item2.Attributes["name"].Value;
				switch (value)
				{
				case "enemy":
				case "enemyextra":
					gameObject.transform.parent = transform4;
					continue;
				case "objectphysics":
				case "objectphysics_item":
					gameObject.transform.parent = transform3;
					continue;
				default:
					if (!value.Contains("cameraevent"))
					{
						switch (value)
						{
						case "eventbox":
						case "vehicle":
						case "wvo_doorA":
						case "wvo_doorB":
						case "dtd_door":
						case "dtd_sandwave":
						case "dtd_pillar_eagle":
						case "dtd_billiard":
						case "dtd_billiardcounter":
						case "dtd_switchcounter":
						case "wap_door":
						case "wap_pathsnowball":
						case "scrollbldg":
						case "csctornado":
						case "csctornadochase":
						case "physicspath":
						case "flc_door":
						case "flamesingle":
						case "flamesequence":
						case "freezedmantle":
						case "flc_flamecore":
						case "rct_door":
						case "normal_train":
						case "freight_train":
						case "eggman_train":
						case "kdv_door":
						case "eagle":
						case "windswitch":
						case "windroad":
						case "robustdoor":
						case "pendulum":
						case "kdv_rainbow":
						case "kdv_decalog":
						case "aqa_door":
						case "aqa_balancer":
						case "changelight":
							break;
						case "particle":
						case "particlesphere":
						case "pointlight":
							gameObject.transform.parent = transform5;
							continue;
						default:
							gameObject.transform.parent = transform2;
							continue;
						}
					}
					break;
				case "player_goal":
				case "switch_collector":
				case "common_laser":
				case "common_object_event":
				case "common_water_collision":
					break;
				}
				gameObject.transform.parent = transform6;
			}
		}
	}

	public static GameObject CreateObject(this XmlNode Object, string Class, XmlNodeList ObjectList)
	{
		string text = "DefaultPrefabs/Objects/";
		bool flag = false;
		XmlNodeList childNodes = Object.ChildNodes;
		switch (Class)
		{
		case "player_start2":
			text += "player_start2";
			break;
		case "player_goal":
			text += "player_goal";
			break;
		case "amigo_collision":
			text = text + "Common/" + Class;
			break;
		case "ring":
		case "spring":
		case "widespring":
		case "dashpanel":
		case "jumppanel":
		case "jumpboard":
		case "pole":
			text = text + "Common/" + Class;
			break;
		case "savepoint":
		case "cameraeventbox":
		case "cameraeventcylinder":
		case "eventbox":
			text = text + "Common/" + Class;
			break;
		case "common_object_event":
			text = text + "Common/" + Class;
			break;
		case "common_stopplayercollision":
			text = text + "Common/" + Class;
			break;
		case "common_psimarksphere":
			text = text + "Common/" + Class;
			break;
		case "common_switch":
		case "common_cage":
		case "common_thorn":
			text = text + "Common/" + Class;
			break;
		case "common_hint_collision":
		case "common_key":
			text = text + "Common/" + Class;
			break;
		case "common_chaosemerald":
			text = text + "Common/" + Class;
			break;
		case "common_laser":
			text = text + "Common/" + Class;
			break;
		case "common_hint":
			text = text + "Common/" + Class;
			break;
		case "common_dashring":
		case "common_rainbowring":
		case "common_guillotine":
			text = text + "Common/" + Class;
			break;
		case "common_warphole":
			text = text + "Common/" + Class;
			break;
		case "common_water_collision":
			text = text + "Common/" + Class;
			break;
		case "common_windcollision_box":
			text = text + "Common/" + Class;
			break;
		case "switch_collector":
			text = text + "Common/" + Class;
			break;
		case "chainjump":
			text = text + "Common/" + Class;
			break;
		case "itemboxa":
			text = text + "Common/" + Class;
			break;
		case "itemboxg":
			text = text + "Common/" + Class;
			break;
		case "updownreel":
			text = text + "Common/" + Class;
			break;
		case "goalring":
		case "snowboardjump":
			text = text + "Common/" + Class;
			break;
		case "medal_of_royal_silver":
			text = text + "Common/" + Class;
			break;
		case "impulsesphere":
			text = text + "Common/" + Class;
			break;
		case "objectphysics":
		{
			string value8 = childNodes[2].Attributes["param"].Value;
			switch (value8)
			{
			case "WoodBox":
			case "WoodBox_Prime":
			case "ReinforcedWoodBox":
			case "BombBox":
			case "BombBox_Prime":
			case "FlashBox":
			case "FlashBox_Prime":
			case "IronBox":
			case "havokthorn":
			case "havokthorn_fix":
				text = text + "Common/" + value8;
				break;
			case "wvo_wvo_obj_cliffA":
			case "wvo_obj_cliffB":
			case "wvo_parasolA":
			case "wvo_parasolB":
			case "wvo_chair":
			case "wvo_table":
			case "wvo_rock":
			case "wvo_rock2":
			case "wvo_cycad":
			case "wvo_foothold":
			case "wvo_bridgeA":
			case "wvo_bridgeA2":
			case "wvo_bridgeB":
			case "wvo_bridgeB2":
			case "guillotine":
				text = text + "WVO/" + value8;
				break;
			case "dtd_pot":
			case "dtd_obj_statue_fix":
			case "dtd_obj_statue":
			case "dtd_obj_statue2":
			case "dtd_pillarC":
			case "dtd_breakwall":
			case "dtd_setbridge":
			case "dtd_setfloor":
			case "dtd_setpole":
			case "dtd_setroad":
				text = text + "DTD/" + value8;
				break;
			case "wap_watchtower":
			case "wap_rock":
			case "wap_rock02":
			case "wap_iceboard":
			case "wap_iceboard_r":
			case "wap_pole":
				text = text + "WAP/" + value8;
				break;
			case "bldgwaste_a":
			case "bldgwaste_b":
			case "csc_breakwall":
			case "csc_breakwall_esp":
			case "csc_breakwall02":
			case "csc_rock":
			case "csc_rock_d":
			case "glass":
			case "csc_tank_base":
			case "csc_tank":
			case "csc_tank2":
			case "csc_tank_d":
			case "taxi":
			case "taxi_br":
			case "taxi_h":
			case "taxi_h_esp":
			case "taxi_d":
			case "sportscar_br":
			case "sportscar_h":
			case "sportscar_h_esp":
			case "sportscar_d":
			case "pickuptruck":
			case "pickuptruck_br":
			case "pickuptruck_d":
			case "pickuptruck_h":
			case "pickuptruck_h_esp":
			case "debrisB":
			case "debrisB_h":
			case "debrisB_br":
			case "groundA":
			case "groundB":
			case "roadA":
			case "streetlamp":
			case "billboard":
			case "container":
			case "container_d":
			case "vent_h":
			case "pipe":
			case "pipe2":
			case "pipe_d":
			case "pipe_h":
			case "obstacleA":
			case "obstacleB":
			case "csc_breakroad":
			case "csc_breakroad_no":
				text = text + "CSC/" + value8;
				break;
			case "flc_mapA_small":
			case "flc_mapA_big":
			case "flc_mapC":
			case "flc_darkmapA":
			case "flc_darkmapC":
			case "flc_darkmapD":
				text = text + "FLC/" + value8;
				break;
			case "rct_barricade":
			case "rct_pillar":
			case "rct_beltbase":
			case "rct_pick":
			case "rct_scoop":
			case "rct_fence":
			case "rct_ladder":
			case "rct_truck":
			case "rct_timber":
			case "rct_barrier":
			case "rct_rock":
				text = text + "RCT/" + value8;
				break;
			case "tpj_obj_oldtree":
			case "tpj_obj_woodwall":
			case "tpj_obj_board":
			case "tpj_obj_gate":
			case "tpj_obj_gate_r":
			case "tpj_obj_gateA":
			case "tpj_obj_gateA_r":
			case "tpj_obj_gateB":
			case "tpj_obj_gateB_r":
			case "tpj_obj_logwall":
			case "tpj_obj_logwall_r":
			case "tpj_obj_ruinA":
			case "tpj_obj_ruinA_r":
			case "tpj_obj_ruinB":
			case "tpj_obj_ruinB_r":
			case "tpj_obj_sphere":
				text = text + "TPJ/" + value8;
				break;
			case "stained":
			case "kdv_rock02":
			case "kdv_rock":
			case "kdv_breakbridge":
			case "kdv_breakbridge_d":
			case "kdv_breaktower":
			case "kdv_scaffold":
			case "kdv_Pillar01":
			case "kdv_Pillar01_br":
			case "kdv_biggate":
			case "kdv_linearch":
				text = text + "KDV/" + value8;
				break;
			case "aqa_foothold":
				text = text + "AQA/" + value8;
				break;
			default:
				text += "Common/ObjUniversal";
				break;
			}
			break;
		}
		case "objectphysics_item":
		{
			string value7 = childNodes[2].Attributes["param"].Value;
			if (value7 == "WoodBox" || value7 == "BombBox")
			{
				text = text + "Common/" + value7;
			}
			break;
		}
		case "physicspath":
			text = text + "Common/" + Class;
			break;
		case "common_path_obj":
		{
			string value6 = childNodes[2].Attributes["param"].Value;
			switch (value6)
			{
			case "WvoSeagull1":
			case "WvoDolphin1":
				text = text + "WVO/" + value6;
				break;
			case "DtdBat1":
			case "DtdBat2":
			case "DtdBat3":
				text = text + "DTD/" + value6;
				break;
			case "TpjButterflyA":
			case "TpjButterflyB":
			case "TpjFlamingo":
				text = text + "TPJ/" + value6;
				break;
			case "AqaManta":
			case "AqaDolphin1":
			case "AqaFishA01":
			case "AqaFishA02":
			case "AqaTurtle":
				text = text + "AQA/" + value6;
				break;
			}
			break;
		}
		case "enemy":
		case "enemyextra":
			text = "DefaultPrefabs/Enemy/" + childNodes[2].Attributes["param"].Value;
			break;
		case "vehicle":
			text = text + "Common/" + Class;
			break;
		case "particle":
		{
			string value4 = childNodes[2].Attributes["param"].Value;
			string value5 = childNodes[3].Attributes["param"].Value;
			switch (value4)
			{
			case "map_dtd":
				switch (value5)
				{
				case "quicksand_05":
				case "torchlight_01":
				case "whlsmoke_01":
				case "whlsmoke_03":
					text = text + "Effects/map_dtd/" + value5;
					break;
				default:
					text += "Common/ObjUniversal";
					break;
				}
				break;
			case "map_wap":
				switch (value5)
				{
				case "crystal_00":
				case "diadust_00":
				case "snowhole01":
				case "snowhole_02":
					text = text + "Effects/map_wap/" + value5;
					break;
				default:
					text += "Common/ObjUniversal";
					break;
				}
				break;
			case "map_csc":
			case "Map_csc":
				switch (value5)
				{
				case "flame1":
				case "flame2":
				case "smoke1":
				case "smoke2":
				case "smoke3":
				case "smoke4":
				case "smoke5":
				case "smoke6":
				case "smoke7":
				case "car_exp1":
				case "car_exp2":
				case "bldg_exp5":
				case "bldg_flame1":
				case "bldg_flame2":
				case "bldg_flame3":
				case "bldg_flame4":
				case "bldg_flame6":
				case "bldg_flame7":
				case "bldg_flame8":
				case "bldg_flame9":
				case "heathaze1":
				case "heathaze2":
				case "heathaze3":
				case "heathaze4":
				case "stome":
				case "firespark2":
				case "firespark3":
				case "firespark5":
				case "stream2":
					text = text + "Effects/map_csc/" + value5;
					break;
				default:
					text += "Common/ObjUniversal";
					break;
				}
				break;
			case "map_flc":
				switch (value5)
				{
				case "spark_01":
				case "prominence_01":
				case "lavsplash_01":
				case "torchlight_01":
				case "torchlight_02":
				case "lavsmoke_01":
				case "lavspark_01":
				case "steam_01":
				case "light_01":
					text = text + "Effects/map_flc/" + value5;
					break;
				default:
					text += "Common/ObjUniversal";
					break;
				}
				break;
			case "map_rct":
				text = ((!(value5 == "mist_01")) ? (text + "Common/ObjUniversal") : (text + "Effects/map_rct/" + value5));
				break;
			case "map_tpj":
				switch (value5)
				{
				case "waterfall_01":
				case "waterfall_02":
				case "waterfall_03":
				case "spore_01":
				case "spore_02":
				case "ripples_01":
					text = text + "Effects/map_tpj/" + value5;
					break;
				default:
					text += "Common/ObjUniversal";
					break;
				}
				break;
			case "map_kdv":
				switch (value5)
				{
				case "torchlight1":
				case "torchlight3":
				case "st_light":
				case "leaf":
				case "w_ripples2":
				case "waterfall6":
					text = text + "Effects/map_kdv/" + value5;
					break;
				default:
					text += "Common/ObjUniversal";
					break;
				}
				break;
			case "map_aqa":
				text = ((!(value5 == "bubble_01")) ? (text + "Common/ObjUniversal") : (text + "Effects/map_aqa/" + value5));
				break;
			}
			break;
		}
		case "particlesphere":
		{
			string value2 = childNodes[2].Attributes["param"].Value;
			string value3 = childNodes[3].Attributes["param"].Value;
			if (value2 == "map_tpj")
			{
				text = ((!(value3 == "leaf_01")) ? (text + "Common/ObjUniversal") : (text + "Effects/map_tpj/" + value3));
			}
			break;
		}
		case "pointlight":
		{
			string value = childNodes[2].Attributes["param"].Value;
			switch (value)
			{
			case "dtdpoint01":
			case "flc_point01":
			case "flc_point02":
			case "flc_point02tz":
			case "flc_point03":
			case "tpj_point01":
			case "kdvpoint01":
			case "aqapointb01":
			case "aqapointr01":
			case "aqapointr02":
			case "aqapointb201":
			case "aqapointb202":
			case "aqapointb203":
			case "aqapointb204":
			case "aqapointb205":
			case "aqapointb206":
				text = text + "Effects/" + value;
				break;
			default:
				text += "Common/ObjUniversal";
				break;
			}
			break;
		}
		case "changelight":
			text = text + "Common/" + Class;
			break;
		case "lightanimation":
			text = text + "Common/" + Class;
			break;
		case "wvo_revolvingnet":
			text = text + "WVO/" + Class;
			break;
		case "wvo_waterslider":
			text = text + "WVO/" + Class;
			break;
		case "wvo_jumpsplinter":
			text = text + "WVO/" + Class;
			break;
		case "wvo_orca":
			text = text + "WVO/" + Class;
			break;
		case "wvo_doorA":
			text = text + "WVO/" + Class;
			break;
		case "wvo_doorB":
			text = text + "WVO/" + Class;
			break;
		case "wvo_battleship":
			text = text + "WVO/" + Class;
			break;
		case "dtd_movingfloor":
			text = text + "DTD/" + Class;
			break;
		case "dtd_pillar":
			text = text + "DTD/" + Class;
			break;
		case "dtd_door":
			text = text + "DTD/" + Class;
			break;
		case "dtd_sandwave":
			text = text + "DTD/" + Class;
			break;
		case "dtd_pillar_eagle":
			text = text + "DTD/" + Class;
			break;
		case "dtd_billiard":
			text = text + "DTD/" + Class;
			break;
		case "dtd_billiardswitch":
			text = text + "DTD/" + Class;
			break;
		case "dtd_billiardcounter":
			text = text + "DTD/" + Class;
			break;
		case "dtd_switchcounter":
			text = text + "DTD/" + Class;
			break;
		case "wap_conifer":
			text = text + "WAP/" + Class;
			break;
		case "wap_door":
			text = text + "WAP/" + Class;
			break;
		case "wap_pathsnowball":
			text = text + "WAP/" + Class;
			break;
		case "wap_brokensnowball":
			text = text + "WAP/" + Class;
			break;
		case "wap_searchlight":
			text = text + "WAP/" + Class;
			break;
		case "cscglass":
			text = text + "CSC/" + Class;
			break;
		case "cscglassbuildbomb":
			text = text + "CSC/" + Class;
			break;
		case "bldgexplode":
			text = text + "CSC/" + Class;
			break;
		case "scrollbldg":
			text = text + "CSC/" + Class;
			break;
		case "csctornado":
			text = text + "CSC/" + Class;
			break;
		case "csctornadochase":
			text = text + "CSC/" + Class;
			break;
		case "ironspring":
			text = text + "CSC/" + Class;
			break;
		case "crater":
			text = text + "FLC/" + Class;
			break;
		case "flamesingle":
			text = text + "FLC/" + Class;
			break;
		case "flamesequence":
			text = text + "FLC/" + Class;
			break;
		case "freezedmantle":
			text = text + "FLC/" + Class;
			break;
		case "inclinedstonebridge":
			text = text + "FLC/" + Class;
			break;
		case "flc_door":
			text = text + "FLC/" + Class;
			break;
		case "flc_volcanicbomb":
			text = text + "FLC/" + Class;
			break;
		case "flc_flamecore":
			text = text + "FLC/" + Class;
			break;
		case "rct_door":
			text = text + "RCT/" + Class;
			break;
		case "rct_belt":
			text = text + "RCT/" + Class;
			break;
		case "rct_seesaw":
			text = text + "RCT/" + Class;
			break;
		case "normal_train":
			text = text + "RCT/" + Class;
			break;
		case "freight_train":
			text = text + "RCT/" + Class;
			break;
		case "eggman_train":
			text = text + "RCT/" + Class;
			break;
		case "lotus":
			text = text + "TPJ/" + Class;
			break;
		case "turtle":
			text = text + "TPJ/" + Class;
			break;
		case "fruit":
			text = text + "TPJ/" + Class;
			break;
		case "bungee":
			text = text + "TPJ/" + Class;
			break;
		case "tarzan":
			text = text + "TPJ/" + Class;
			break;
		case "espswing":
			text = text + "TPJ/" + Class;
			break;
		case "hangingrock":
			text = text + "TPJ/" + Class;
			break;
		case "brickwall":
			text = text + "KDV/" + Class;
			break;
		case "eagle":
			text = text + "KDV/" + Class;
			break;
		case "rope":
			text = text + "KDV/" + Class;
			break;
		case "inclinedbridge":
			text = text + "KDV/" + Class;
			break;
		case "windswitch":
			text = text + "KDV/" + Class;
			break;
		case "windroad":
			text = text + "KDV/" + Class;
			break;
		case "brokenstairs_right":
			text = text + "KDV/" + Class;
			break;
		case "brokenstairs_left":
			text = text + "KDV/" + Class;
			break;
		case "espstairs_right":
			text = text + "KDV/" + Class;
			break;
		case "espstairs_left":
			text = text + "KDV/" + Class;
			break;
		case "gate":
			text = text + "KDV/" + Class;
			break;
		case "robustdoor":
			text = text + "KDV/" + Class;
			break;
		case "pendulum":
			text = text + "KDV/" + Class;
			break;
		case "kdv_door":
			text = text + "KDV/" + Class;
			break;
		case "kdv_rainbow":
			text = text + "KDV/" + Class;
			break;
		case "kdv_decalog":
			text = text + "KDV/" + Class;
			break;
		case "aqa_door":
			text = text + "AQA/" + Class;
			break;
		case "aqa_lamp":
			text = text + "AQA/" + Class;
			break;
		case "aqa_balancer":
			text = text + "AQA/" + Class;
			break;
		case "aqa_magnet":
			text = text + "AQA/" + Class;
			break;
		case "aqa_mercury_small":
			text = text + "AQA/" + Class;
			break;
		case "aqa_pond":
			text = text + "AQA/" + Class;
			break;
		case "aqa_launcher":
			text = text + "AQA/" + Class;
			break;
		case "aqa_glass_blue":
			text = text + "AQA/" + Class;
			break;
		case "aqa_glass_red":
			text = text + "AQA/" + Class;
			break;
		case "townsgoal":
			text = text + "TWN/" + Class;
			break;
		case "warpgate":
			text = text + "TWN/" + Class;
			break;
		default:
			flag = true;
			break;
		}
		if (flag)
		{
			return null;
		}
		GameObject gameObject = text.InstantiateResource();
		if (gameObject == null)
		{
			return null;
		}
		gameObject.transform.position = PositionParam(childNodes[0]);
		gameObject.transform.rotation = RotationParam(childNodes[1]);
		gameObject.transform.RotateAround(Vector3.zero, Vector3.up, 90f);
		if (Class == "player_start2")
		{
			int player_No = int.Parse(childNodes[2].Attributes["param"].Value);
			string value9 = childNodes[3].Attributes["param"].Value;
			bool amigo = childNodes[4].Attributes["param"].Value == "1";
			gameObject.GetComponent<PlayerStart>().SetParameters(player_No, value9, amigo);
		}
		if (Class == "player_goal")
		{
			Vector3 cam_Pos = VectorParam(childNodes[2], RotatePos: false);
			Vector3 cam_Tgt = VectorParam(childNodes[3], RotatePos: false);
			gameObject.GetComponent<PlayerGoal>().SetParameters(cam_Pos, cam_Tgt);
		}
		if (Class == "amigo_collision")
		{
			float z = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float x = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float y = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			string value10 = childNodes[5].Attributes["param"].Value;
			Vector3 target = Vector3.zero;
			if (value10 != "4294967295")
			{
				target = PositionParam(ObjectList[int.Parse(value10)].ChildNodes[0], RotatePos: false);
			}
			bool chase = childNodes[6].Attributes["param"].Value == "1";
			BoxCollider component = gameObject.GetComponent<BoxCollider>();
			component.size = new Vector3(x, y, z);
			component.center = new Vector3(0f, component.size.y * 0.5f, 0f);
			gameObject.GetComponent<AmigoCollision>().SetParameters(target, chase);
		}
		if (Class == "ring")
		{
			bool groundLightDash = childNodes[2].Attributes["param"].Value == "1";
			float splineTime = float.Parse(childNodes[3].Attributes["param"].Value);
			string value11 = childNodes[4].Attributes["param"].Value;
			gameObject.GetComponent<Ring>().SetParameters(groundLightDash, splineTime, value11);
			gameObject.transform.position += Vector3.up * 0.5f;
		}
		if (Class == "spring")
		{
			float speed = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float timer = float.Parse(childNodes[3].Attributes["param"].Value);
			string value12 = childNodes[4].Attributes["param"].Value;
			Vector3 targetPosition = Vector3.zero;
			if (value12 != "4294967295")
			{
				targetPosition = PositionParam(ObjectList[int.Parse(value12)].ChildNodes[0], RotatePos: false);
			}
			gameObject.GetComponent<Spring>().SetParameters(speed, timer, targetPosition);
		}
		if (Class == "widespring")
		{
			float speed2 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float timer2 = float.Parse(childNodes[3].Attributes["param"].Value) / 2f;
			gameObject.GetComponent<Widespring>().SetParameters(speed2, timer2);
		}
		if (Class == "dashpanel")
		{
			float speed3 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float timer3 = float.Parse(childNodes[3].Attributes["param"].Value);
			gameObject.GetComponent<DashPanel>().SetParameters(speed3, timer3);
		}
		if (Class == "jumppanel")
		{
			float yOffset = float.Parse(childNodes[2].Attributes["param"].Value);
			float speed4 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float timer4 = float.Parse(childNodes[4].Attributes["param"].Value);
			string value13 = childNodes[5].Attributes["param"].Value;
			Vector3 targetPosition2 = Vector3.zero;
			if (value13 != "4294967295")
			{
				targetPosition2 = PositionParam(ObjectList[int.Parse(value13)].ChildNodes[0], RotatePos: false);
			}
			gameObject.GetComponent<JumpPanel>().SetParameters(yOffset, speed4, timer4, targetPosition2);
		}
		if (Class == "jumpboard")
		{
			float yOffset2 = float.Parse(childNodes[2].Attributes["param"].Value);
			float speed5 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float timer5 = float.Parse(childNodes[4].Attributes["param"].Value);
			string value14 = childNodes[5].Attributes["param"].Value;
			Vector3 targetPosition3 = Vector3.zero;
			if (value14 != "4294967295")
			{
				targetPosition3 = PositionParam(ObjectList[int.Parse(value14)].ChildNodes[0], RotatePos: false);
			}
			gameObject.GetComponent<JumpBoard>().SetParameters(yOffset2, speed5, timer5, targetPosition3);
		}
		if (Class == "pole")
		{
			float radius = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float height = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float power = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			float pitch = float.Parse(childNodes[5].Attributes["param"].Value);
			float yaw = float.Parse(childNodes[6].Attributes["param"].Value);
			float rotation_Time = float.Parse(childNodes[7].Attributes["param"].Value);
			float out_Time = float.Parse(childNodes[8].Attributes["param"].Value);
			gameObject.GetComponent<Pole>().SetParameters(radius, height, power, pitch, yaw, rotation_Time, out_Time);
		}
		if (Class == "cameraeventbox")
		{
			float z2 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float x2 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float y2 = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			int mode = int.Parse(childNodes[7].Attributes["param"].Value);
			Vector3 position = VectorParam(childNodes[8], RotatePos: false);
			Vector3 target2 = VectorParam(childNodes[9], RotatePos: false);
			BoxCollider component2 = gameObject.GetComponent<BoxCollider>();
			component2.size = new Vector3(x2, y2, z2);
			component2.center = new Vector3(0f, component2.size.y * 0.5f, 0f);
			gameObject.GetComponent<CameraEvents>().cameraParameters = new CameraParameters(mode, position, target2);
		}
		if (Class == "cameraeventcylinder")
		{
			float num = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float num2 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			int mode2 = int.Parse(childNodes[6].Attributes["param"].Value);
			Vector3 position2 = VectorParam(childNodes[7], RotatePos: false);
			Vector3 target3 = VectorParam(childNodes[8], RotatePos: false);
			gameObject.transform.position += Vector3.up * num2 * 0.5f;
			gameObject.transform.localScale = new Vector3(num, num2, num);
			gameObject.GetComponent<CameraEvents>().cameraParameters = new CameraParameters(mode2, position2, target3);
		}
		if (Class == "eventbox")
		{
			float z3 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float x3 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float y3 = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			string value15 = childNodes[5].Attributes["param"].Value;
			BoxCollider component3 = gameObject.GetComponent<BoxCollider>();
			component3.size = new Vector3(x3, y3, z3);
			component3.center = new Vector3(0f, component3.size.y * 0.5f, 0f);
			gameObject.GetComponent<EventBox>().SetParameters(value15);
		}
		if (Class == "common_object_event")
		{
			float z4 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float x4 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float y4 = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			string value16 = childNodes[5].Attributes["param"].Value;
			BoxCollider component4 = gameObject.GetComponent<BoxCollider>();
			component4.size = new Vector3(x4, y4, z4);
			component4.center = new Vector3(0f, component4.size.y * 0.5f, 0f);
			gameObject.GetComponent<Common_Object_Event>().SetParameters(value16);
		}
		if (Class == "common_stopplayercollision")
		{
			float z5 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float x5 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float y5 = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			BoxCollider component5 = gameObject.GetComponent<BoxCollider>();
			component5.size = new Vector3(x5, y5, z5);
			component5.center = new Vector3(0f, component5.size.y * 0.5f, 0f);
		}
		if (Class == "common_psimarksphere")
		{
			float radius2 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			string value17 = childNodes[3].Attributes["param"].Value;
			gameObject.GetComponent<Common_PsiMarkSphere>().SetParameters(radius2, value17);
		}
		if (Class == "common_switch")
		{
			int type = int.Parse(childNodes[2].Attributes["param"].Value);
			string value18 = childNodes[3].Attributes["param"].Value;
			string value19 = childNodes[4].Attributes["param"].Value;
			float time = float.Parse(childNodes[5].Attributes["param"].Value);
			string value20 = childNodes[6].Attributes["param"].Value;
			Vector3 target4 = Vector3.zero;
			if (value20 != "4294967295")
			{
				target4 = PositionParam(ObjectList[int.Parse(value20)].ChildNodes[0], RotatePos: false);
			}
			gameObject.GetComponent<Common_Switch>().SetParameters(type, value18, value19, time, target4);
		}
		if (Class == "common_cage")
		{
			float parameters = float.Parse(childNodes[2].Attributes["param"].Value);
			gameObject.GetComponent<Common_Cage>().SetParameters(parameters);
		}
		if (Class == "common_thorn")
		{
			int type2 = int.Parse(childNodes[2].Attributes["param"].Value);
			float outTime = float.Parse(childNodes[3].Attributes["param"].Value);
			float inTime = float.Parse(childNodes[4].Attributes["param"].Value);
			gameObject.GetComponent<Common_Thorn>().SetParameters(type2, outTime, inTime);
		}
		if (Class == "common_hint_collision")
		{
			float z6 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float x6 = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			float y6 = float.Parse(childNodes[5].Attributes["param"].Value) * Scale;
			BoxCollider component6 = gameObject.GetComponent<BoxCollider>();
			component6.size = new Vector3(x6, y6, z6);
			component6.center = new Vector3(0f, component6.size.y * 0.5f, 0f);
			gameObject.GetComponent<Common_Hint_Collison>().SetParameters(childNodes[2].Attributes["param"].Value);
		}
		if (Class == "common_key")
		{
			string value21 = childNodes[2].Attributes["param"].Value;
			gameObject.GetComponent<Common_Key>().SetParameters(value21);
		}
		if (Class == "common_chaosemerald")
		{
			int parameters2 = int.Parse(childNodes[2].Attributes["param"].Value);
			gameObject.GetComponent<Common_ChaosEmerald>().SetParameters(parameters2);
		}
		if (Class == "common_laser")
		{
			string value22 = childNodes[3].Attributes["param"].Value;
			Vector3 pair = Vector3.zero;
			if (value22 != "4294967295")
			{
				pair = PositionParam(ObjectList[int.Parse(value22)].ChildNodes[0], RotatePos: false);
			}
			int num3 = int.Parse(childNodes[4].Attributes["param"].Value);
			float interval = float.Parse(childNodes[5].Attributes["param"].Value) * Scale;
			float onTime = float.Parse(childNodes[6].Attributes["param"].Value);
			float offTime = float.Parse(childNodes[7].Attributes["param"].Value);
			float range = float.Parse(childNodes[8].Attributes["param"].Value) * Scale;
			float speed6 = float.Parse(childNodes[9].Attributes["param"].Value) * Scale;
			float radRange = float.Parse(childNodes[10].Attributes["param"].Value);
			float radSpeed = float.Parse(childNodes[11].Attributes["param"].Value);
			gameObject.GetComponent<Common_Laser>().SetParameters(pair, num3, interval, onTime, offTime, range, speed6, radRange, radSpeed);
		}
		if (Class == "common_hint")
		{
			gameObject.GetComponent<Common_Hint>().SetParameters(childNodes[2].Attributes["param"].Value);
		}
		if (Class == "common_dashring")
		{
			float speed7 = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			float timer6 = float.Parse(childNodes[5].Attributes["param"].Value) * Scale;
			gameObject.GetComponent<Common_DashRing>().SetParameters(speed7, timer6);
		}
		if (Class == "common_rainbowring")
		{
			float inclination = float.Parse(childNodes[2].Attributes["param"].Value);
			float speed8 = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			float timer7 = float.Parse(childNodes[5].Attributes["param"].Value);
			gameObject.GetComponent<Common_RainbowRing>().SetParameters(inclination, speed8, timer7);
		}
		if (Class == "common_guillotine")
		{
			int length = int.Parse(childNodes[2].Attributes["param"].Value);
			int num4 = int.Parse(childNodes[3].Attributes["param"].Value);
			float upWidth = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			float downWidth = float.Parse(childNodes[5].Attributes["param"].Value) * Scale;
			float time2 = float.Parse(childNodes[6].Attributes["param"].Value);
			float speed9 = float.Parse(childNodes[7].Attributes["param"].Value) * Scale;
			float distance = float.Parse(childNodes[8].Attributes["param"].Value) * Scale;
			gameObject.GetComponent<Common_Guillotine>().SetParameters(length, num4, upWidth, downWidth, time2, speed9, distance);
		}
		if (Class == "common_warphole")
		{
			string value23 = childNodes[3].Attributes["param"].Value;
			Vector3 target5 = VectorParam(childNodes[4], RotatePos: false);
			gameObject.GetComponent<Common_WarpHole>().SetParameters(value23, target5);
		}
		if (Class == "common_water_collision")
		{
			float z7 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float x7 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float y7 = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			BoxCollider component7 = gameObject.GetComponent<BoxCollider>();
			component7.size = new Vector3(x7, y7, z7);
			component7.center = new Vector3(0f, component7.size.y * 0.5f, 0f);
		}
		if (Class == "common_windcollision_box")
		{
			float z8 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float x8 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float y8 = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			float power2 = float.Parse(childNodes[5].Attributes["param"].Value);
			float floatHeight = float.Parse(childNodes[6].Attributes["param"].Value) * Scale;
			int particle = int.Parse(childNodes[7].Attributes["param"].Value);
			BoxCollider component8 = gameObject.GetComponent<BoxCollider>();
			component8.size = new Vector3(x8, y8, z8);
			component8.center = new Vector3(0f, component8.size.y * 0.5f, 0f);
			gameObject.GetComponent<WindCollisionBox>().SetParameters(power2, floatHeight, particle);
		}
		if (Class == "switch_collector")
		{
			int count = int.Parse(childNodes[2].Attributes["param"].Value);
			string value24 = childNodes[3].Attributes["param"].Value;
			Vector3 target6 = Vector3.zero;
			if (value24 != "4294967295")
			{
				target6 = PositionParam(ObjectList[int.Parse(value24)].ChildNodes[0], RotatePos: false);
			}
			string value25 = childNodes[4].Attributes["param"].Value;
			Vector3 target7 = Vector3.zero;
			if (value25 != "4294967295")
			{
				target7 = PositionParam(ObjectList[int.Parse(value25)].ChildNodes[0], RotatePos: false);
			}
			string value26 = childNodes[5].Attributes["param"].Value;
			Vector3 target8 = Vector3.zero;
			if (value26 != "4294967295")
			{
				target8 = PositionParam(ObjectList[int.Parse(value26)].ChildNodes[0], RotatePos: false);
			}
			string value27 = childNodes[6].Attributes["param"].Value;
			Vector3 target9 = Vector3.zero;
			if (value27 != "4294967295")
			{
				target9 = PositionParam(ObjectList[int.Parse(value27)].ChildNodes[0], RotatePos: false);
			}
			string value28 = childNodes[7].Attributes["param"].Value;
			Vector3 target10 = Vector3.zero;
			if (value28 != "4294967295")
			{
				target10 = PositionParam(ObjectList[int.Parse(value28)].ChildNodes[0], RotatePos: false);
			}
			string value29 = childNodes[8].Attributes["param"].Value;
			Vector3 target11 = Vector3.zero;
			if (value29 != "4294967295")
			{
				target11 = PositionParam(ObjectList[int.Parse(value29)].ChildNodes[0], RotatePos: false);
			}
			string value30 = childNodes[9].Attributes["param"].Value;
			Vector3 target12 = Vector3.zero;
			if (value30 != "4294967295")
			{
				target12 = PositionParam(ObjectList[int.Parse(value30)].ChildNodes[0], RotatePos: false);
			}
			string value31 = childNodes[10].Attributes["param"].Value;
			Vector3 target13 = Vector3.zero;
			if (value31 != "4294967295")
			{
				target13 = PositionParam(ObjectList[int.Parse(value31)].ChildNodes[0], RotatePos: false);
			}
			string value32 = childNodes[11].Attributes["param"].Value;
			Vector3 target14 = Vector3.zero;
			if (value32 != "4294967295")
			{
				target14 = PositionParam(ObjectList[int.Parse(value32)].ChildNodes[0], RotatePos: false);
			}
			string value33 = childNodes[12].Attributes["param"].Value;
			gameObject.GetComponent<Switch_Collector>().SetParameters(count, target6, target7, target8, target9, target10, target11, target12, target13, target14, value33);
		}
		if (Class == "chainjump")
		{
			string value34 = childNodes[2].Attributes["param"].Value;
			Vector3 parameters3 = Vector3.zero;
			if (value34 != "4294967295")
			{
				parameters3 = PositionParam(ObjectList[int.Parse(value34)].ChildNodes[0], RotatePos: false);
			}
			gameObject.GetComponent<ChainJump>().SetParameters(parameters3);
		}
		if (Class == "itemboxa" || Class == "itemboxg")
		{
			gameObject.GetComponent<ItemBox>().SetParameters(int.Parse(childNodes[2].Attributes["param"].Value));
		}
		if (Class == "updownreel")
		{
			float height2 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float height3 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float time3 = float.Parse(childNodes[4].Attributes["param"].Value);
			gameObject.GetComponent<UpDownReel>().SetParameters(height2, height3, time3);
		}
		if (Class == "snowboardjump")
		{
			float z9 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float x9 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float y9 = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			float power3 = float.Parse(childNodes[5].Attributes["param"].Value) * Scale;
			float pitch2 = float.Parse(childNodes[6].Attributes["param"].Value);
			float rate = float.Parse(childNodes[7].Attributes["param"].Value);
			float bPower_Rate = float.Parse(childNodes[8].Attributes["param"].Value);
			float time4 = float.Parse(childNodes[9].Attributes["param"].Value);
			BoxCollider component9 = gameObject.GetComponent<BoxCollider>();
			component9.size = new Vector3(x9, y9, z9);
			component9.center = new Vector3(0f, component9.size.y * 0.5f, 0f);
			gameObject.GetComponent<SnowBoardJump>().SetParameters(power3, pitch2, rate, bPower_Rate, time4);
		}
		if (Class == "medal_of_royal_silver")
		{
			string value35 = childNodes[2].Attributes["param"].Value;
			gameObject.GetComponent<MedalOfRoyal_Silver>().SetParameters(value35);
		}
		if (Class == "impulsesphere")
		{
			float radius3 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float impulse = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			gameObject.GetComponent<ImpulseSphere>().SetParameters(radius3, impulse);
		}
		if (Class == "objectphysics" || Class == "objectphysics_item")
		{
			switch (childNodes[2].Attributes["param"].Value)
			{
			case "WoodBox":
			case "ReinforcedWoodBox":
			case "BombBox":
			case "BombBox_Prime":
			case "FlashBox":
			case "FlashBox_Prime":
			case "IronBox":
				gameObject.transform.position += Vector3.up * 1.05f;
				break;
			}
			if (Class == "objectphysics_item")
			{
				int dropRate = int.Parse(childNodes[4].Attributes["param"].Value);
				int itemType = int.Parse(childNodes[5].Attributes["param"].Value);
				gameObject.AddComponent<DropItems>().SetParameters(dropRate, itemType);
			}
		}
		if (Class == "physicspath")
		{
			string value36 = childNodes[2].Attributes["param"].Value;
			string value37 = childNodes[4].Attributes["param"].Value;
			float speed10 = float.Parse(childNodes[5].Attributes["param"].Value) * Scale;
			gameObject.GetComponent<PhysicsPath>().SetParameters(value36, value37, speed10);
		}
		if (Class == "common_path_obj")
		{
			string value38 = childNodes[3].Attributes["param"].Value;
			float speed11 = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			gameObject.GetComponent<SplineMovement>().SetParameters(speed11, value38);
		}
		if (Class == "enemy")
		{
			string value39 = childNodes[2].Attributes["param"].Value;
			string value40 = childNodes[4].Attributes["param"].Value;
			switch (value39)
			{
			case "eBomber":
			case "eSweeper":
			case "eArmor":
				gameObject.GetComponent<eBomber>().RobotMode = (eBomber.Mode)Enum.Parse(typeof(eBomber.Mode), GetEnemyString(value40));
				break;
			}
			if (value39 == "eCannon" || value39 == "eWalker")
			{
				gameObject.GetComponent<eCannon>().RobotMode = (eCannon.Mode)Enum.Parse(typeof(eCannon.Mode), GetEnemyString(value40));
			}
			if (value39 == "eCannon(Fly)")
			{
				gameObject.GetComponent<eCannonFly>().RobotMode = (eCannonFly.Mode)Enum.Parse(typeof(eCannonFly.Mode), GetEnemyString(value40));
			}
			if (value39 == "eFlyer" || value39 == "eBluster")
			{
				gameObject.GetComponent<eFlyer>().RobotMode = (eFlyer.Mode)Enum.Parse(typeof(eFlyer.Mode), GetEnemyString(value40));
			}
			if (value39 == "eGuardian" || value39 == "eKeeper")
			{
				gameObject.GetComponent<eGuardian>().RobotMode = (eGuardian.Mode)Enum.Parse(typeof(eGuardian.Mode), GetEnemyString(value40));
			}
			switch (value39)
			{
			case "eGunner":
			case "eStinger":
			case "eLancer":
			case "eBuster":
				gameObject.GetComponent<eGunner>().RobotMode = (eGunner.Mode)Enum.Parse(typeof(eGunner.Mode), GetEnemyString(value40));
				break;
			}
			switch (value39)
			{
			case "eGunner(Fly)":
			case "eStinger(Fly)":
			case "eLancer(Fly)":
			case "eBuster(Fly)":
				gameObject.GetComponent<eGunnerFly>().RobotMode = (eGunnerFly.Mode)Enum.Parse(typeof(eGunnerFly.Mode), GetEnemyString(value40));
				break;
			}
			if (value39 == "eLiner" || value39 == "eChaser")
			{
				string parameters4 = childNodes[5].Attributes["param"].Value.Split("."[0])[0];
				gameObject.GetComponent<eLiner>().SetParameters(parameters4);
				gameObject.GetComponent<eLiner>().RobotMode = (eLiner.Mode)Enum.Parse(typeof(eLiner.Mode), GetEnemyString(value40));
			}
			if (value39 == "eRounder" || value39 == "eCommander")
			{
				string parameters5 = childNodes[5].Attributes["param"].Value.Split("."[0])[0];
				gameObject.GetComponent<eRounder>().SetParameters(parameters5);
				gameObject.GetComponent<eRounder>().RobotMode = (eRounder.Mode)Enum.Parse(typeof(eRounder.Mode), GetEnemyString(value40));
			}
			if (value39 == "eSearcher" || value39 == "eHunter")
			{
				gameObject.GetComponent<eSearcher>().RobotMode = (eSearcher.Mode)Enum.Parse(typeof(eSearcher.Mode), GetEnemyString(value40));
			}
			if (value39 == "cBiter" || value39 == "cStalker")
			{
				gameObject.GetComponent<cBiter>().CreatureMode = (cBiter.Mode)Enum.Parse(typeof(cBiter.Mode), GetEnemyString(value40));
			}
			if (value39 == "cCrawler" || value39 == "cGazer")
			{
				gameObject.GetComponent<cCrawler>().CreatureMode = (cCrawler.Mode)Enum.Parse(typeof(cCrawler.Mode), GetEnemyString(value40));
			}
			if (value39 == "cGolem" || value39 == "cTitan")
			{
				gameObject.GetComponent<cGolem>().CreatureMode = (cGolem.Mode)Enum.Parse(typeof(cGolem.Mode), GetEnemyString(value40));
			}
			if (value39 == "cTaker" || value39 == "cTricker")
			{
				gameObject.GetComponent<cTaker>().CreatureMode = (cTaker.Mode)Enum.Parse(typeof(cTaker.Mode), GetEnemyString(value40));
			}
		}
		if (Class == "enemyextra")
		{
			string value41 = childNodes[2].Attributes["param"].Value;
			string value42 = childNodes[4].Attributes["param"].Value;
			bool findPlayer = childNodes[7].Attributes["param"].Value == "1";
			bool isFixed = childNodes[9].Attributes["param"].Value == "1";
			string value43 = childNodes[10].Attributes["param"].Value;
			float appearVelocity = float.Parse(childNodes[11].Attributes["param"].Value) * Scale;
			string value44 = childNodes[13].Attributes["param"].Value;
			string value45 = childNodes[15].Attributes["param"].Value;
			Vector3 homingTarget = Vector3.zero;
			if (value45 != "4294967295")
			{
				homingTarget = PositionParam(ObjectList[int.Parse(value45)].ChildNodes[0], RotatePos: false);
			}
			switch (value41)
			{
			case "eBomber":
			case "eSweeper":
			case "eArmor":
				gameObject.GetComponent<eBomber>().RobotMode = (eBomber.Mode)Enum.Parse(typeof(eBomber.Mode), GetEnemyString(value42));
				break;
			}
			if (value41 == "eCannon" || value41 == "eWalker")
			{
				gameObject.GetComponent<eCannon>().RobotMode = (eCannon.Mode)Enum.Parse(typeof(eCannon.Mode), GetEnemyString(value42));
			}
			if (value41 == "eCannon(Fly)")
			{
				gameObject.GetComponent<eCannonFly>().RobotMode = (eCannonFly.Mode)Enum.Parse(typeof(eCannonFly.Mode), GetEnemyString(value42));
			}
			if (value41 == "eFlyer" || value41 == "eBluster")
			{
				gameObject.GetComponent<eFlyer>().RobotMode = (eFlyer.Mode)Enum.Parse(typeof(eFlyer.Mode), GetEnemyString(value42));
			}
			if (value41 == "eGuardian" || value41 == "eKeeper")
			{
				gameObject.GetComponent<eGuardian>().RobotMode = (eGuardian.Mode)Enum.Parse(typeof(eGuardian.Mode), GetEnemyString(value42));
			}
			switch (value41)
			{
			case "eGunner":
			case "eStinger":
			case "eLancer":
			case "eBuster":
				gameObject.GetComponent<eGunner>().RobotMode = (eGunner.Mode)Enum.Parse(typeof(eGunner.Mode), GetEnemyString(value42));
				break;
			}
			switch (value41)
			{
			case "eGunner(Fly)":
			case "eStinger(Fly)":
			case "eLancer(Fly)":
			case "eBuster(Fly)":
				gameObject.GetComponent<eGunnerFly>().RobotMode = (eGunnerFly.Mode)Enum.Parse(typeof(eGunnerFly.Mode), GetEnemyString(value42));
				break;
			}
			if (value41 == "eLiner" || value41 == "eChaser")
			{
				string parameters6 = childNodes[5].Attributes["param"].Value.Split("."[0])[0];
				gameObject.GetComponent<eLiner>().SetParameters(parameters6);
				gameObject.GetComponent<eLiner>().RobotMode = (eLiner.Mode)Enum.Parse(typeof(eLiner.Mode), GetEnemyString(value42));
			}
			if (value41 == "eRounder" || value41 == "eCommander")
			{
				string parameters7 = childNodes[5].Attributes["param"].Value.Split("."[0])[0];
				gameObject.GetComponent<eRounder>().SetParameters(parameters7);
				gameObject.GetComponent<eRounder>().RobotMode = (eRounder.Mode)Enum.Parse(typeof(eRounder.Mode), GetEnemyString(value42));
			}
			if (value41 == "eSearcher" || value41 == "eHunter")
			{
				gameObject.GetComponent<eSearcher>().RobotMode = (eSearcher.Mode)Enum.Parse(typeof(eSearcher.Mode), GetEnemyString(value42));
			}
			if (value41 == "cBiter" || value41 == "cStalker")
			{
				gameObject.GetComponent<cBiter>().CreatureMode = (cBiter.Mode)Enum.Parse(typeof(cBiter.Mode), GetEnemyString(value42));
			}
			if (value41 == "cCrawler" || value41 == "cGazer")
			{
				gameObject.GetComponent<cCrawler>().CreatureMode = (cCrawler.Mode)Enum.Parse(typeof(cCrawler.Mode), GetEnemyString(value42));
			}
			if (value41 == "cGolem" || value41 == "cTitan")
			{
				gameObject.GetComponent<cGolem>().CreatureMode = (cGolem.Mode)Enum.Parse(typeof(cGolem.Mode), GetEnemyString(value42));
			}
			if (value41 == "cTaker" || value41 == "cTricker")
			{
				gameObject.GetComponent<cTaker>().CreatureMode = (cTaker.Mode)Enum.Parse(typeof(cTaker.Mode), GetEnemyString(value42));
			}
			gameObject.GetComponent<EnemyBase>().SetParameters(findPlayer, isFixed, value43, appearVelocity, value44, homingTarget);
		}
		if (Class == "vehicle")
		{
			int type3 = int.Parse(childNodes[2].Attributes["param"].Value);
			bool isGetOut = childNodes[4].Attributes["param"].Value == "0";
			bool isShoot = childNodes[5].Attributes["param"].Value == "0";
			gameObject.GetComponent<Vehicle>().SetParameters(type3, isGetOut, isShoot);
		}
		if (Class == "changelight")
		{
			float z10 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float x10 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float y10 = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			string value46 = childNodes[5].Attributes["param"].Value;
			string value47 = childNodes[6].Attributes["param"].Value;
			string value48 = childNodes[7].Attributes["param"].Value;
			BoxCollider component10 = gameObject.GetComponent<BoxCollider>();
			component10.size = new Vector3(x10, y10, z10);
			component10.center = new Vector3(0f, component10.size.y * 0.5f, 0f);
			gameObject.GetComponent<ChangeLight>().SetParameters(value46, value47, value48);
		}
		if (Class == "lightanimation")
		{
			float z11 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float x11 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float y11 = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			string value49 = childNodes[5].Attributes["param"].Value;
			string value50 = childNodes[6].Attributes["param"].Value;
			string value51 = childNodes[7].Attributes["param"].Value;
			BoxCollider component11 = gameObject.GetComponent<BoxCollider>();
			component11.size = new Vector3(x11, y11, z11);
			component11.center = new Vector3(0f, component11.size.y * 0.5f, 0f);
			gameObject.GetComponent<LightAnimation>().SetParameters(value49, value50, value51);
		}
		if (Class == "wvo_waterslider")
		{
			string value52 = childNodes[2].Attributes["param"].Value;
			float radius4 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			gameObject.GetComponent<WaterSlider>().SetParameters(value52, radius4);
		}
		if (Class == "wvo_jumpsplinter")
		{
			string value53 = childNodes[2].Attributes["param"].Value;
			Vector3 zero = Vector3.zero;
			zero = PositionParam(ObjectList[int.Parse(value53)].ChildNodes[0], RotatePos: false);
			gameObject.GetComponent<JumpSplinter>().SetParameters(zero);
		}
		if (Class == "wvo_orca")
		{
			int type4 = int.Parse(childNodes[2].Attributes["param"].Value);
			float speed12 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			string value54 = childNodes[4].Attributes["param"].Value;
			gameObject.GetComponent<Orca>().SetParameters(type4, speed12, value54);
		}
		if (Class == "wvo_doorA" || Class == "wvo_doorB")
		{
			bool signal = childNodes[2].Attributes["param"].Value == "1";
			int mode3 = int.Parse(childNodes[6].Attributes["param"].Value);
			gameObject.GetComponent<WvoDoor>().SetParameters(signal, mode3);
		}
		if (Class == "wvo_battleship")
		{
			string value55 = childNodes[2].Attributes["param"].Value;
			float distance2 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			gameObject.GetComponent<BattleShip>().SetParameters(value55, distance2);
		}
		if (Class == "dtd_movingfloor")
		{
			float on_Time = float.Parse(childNodes[2].Attributes["param"].Value);
			float off_Time = float.Parse(childNodes[3].Attributes["param"].Value);
			float appear_Time = float.Parse(childNodes[4].Attributes["param"].Value);
			float disappear_Time = float.Parse(childNodes[5].Attributes["param"].Value);
			gameObject.GetComponent<MovingFloor>().SetParameters(on_Time, off_Time, appear_Time, disappear_Time);
		}
		if (Class == "dtd_pillar")
		{
			float height4 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float time5 = float.Parse(childNodes[3].Attributes["param"].Value);
			int type5 = int.Parse(childNodes[4].Attributes["param"].Value);
			bool noEffect = childNodes[5].Attributes["param"].Value == "1";
			gameObject.GetComponent<PillarA>().SetParameters(height4, time5, type5, noEffect);
		}
		if (Class == "dtd_sandwave")
		{
			float parameters8 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			gameObject.GetComponent<SandWave>().SetParameters(parameters8);
		}
		if (Class == "dtd_pillar_eagle")
		{
			float height5 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float time6 = float.Parse(childNodes[3].Attributes["param"].Value);
			bool noEffect2 = childNodes[4].Attributes["param"].Value == "1";
			gameObject.GetComponent<PillarB>().SetParameters(height5, time6, noEffect2);
		}
		if (Class == "dtd_billiard")
		{
			int parameters9 = int.Parse(childNodes[2].Attributes["param"].Value);
			gameObject.GetComponent<Dtd_Billiard>().SetParameters(parameters9);
		}
		if (Class == "dtd_billiardswitch")
		{
			float z12 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float x12 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float y12 = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			BoxCollider component12 = gameObject.GetComponent<BoxCollider>();
			component12.size = new Vector3(x12, y12, z12);
			component12.center = new Vector3(0f, component12.size.y * 0.5f, 0f);
			string value56 = childNodes[5].Attributes["param"].Value;
			gameObject.GetComponent<Dtd_BilliardSwitch>().SetParameters(value56);
		}
		if (Class == "dtd_billiardcounter")
		{
			int count2 = int.Parse(childNodes[2].Attributes["param"].Value);
			string value57 = childNodes[4].Attributes["param"].Value;
			Vector3 target15 = Vector3.zero;
			if (value57 != "4294967295")
			{
				target15 = PositionParam(ObjectList[int.Parse(value57)].ChildNodes[0], RotatePos: false);
			}
			string value58 = childNodes[5].Attributes["param"].Value;
			Vector3 target16 = Vector3.zero;
			if (value58 != "4294967295")
			{
				target16 = PositionParam(ObjectList[int.Parse(value58)].ChildNodes[0], RotatePos: false);
			}
			string value59 = childNodes[6].Attributes["param"].Value;
			Vector3 target17 = Vector3.zero;
			if (value59 != "4294967295")
			{
				target17 = PositionParam(ObjectList[int.Parse(value59)].ChildNodes[0], RotatePos: false);
			}
			string value60 = childNodes[7].Attributes["param"].Value;
			Vector3 target18 = Vector3.zero;
			if (value60 != "4294967295")
			{
				target18 = PositionParam(ObjectList[int.Parse(value60)].ChildNodes[0], RotatePos: false);
			}
			string value61 = childNodes[8].Attributes["param"].Value;
			Vector3 target19 = Vector3.zero;
			if (value61 != "4294967295")
			{
				target19 = PositionParam(ObjectList[int.Parse(value61)].ChildNodes[0], RotatePos: false);
			}
			string value62 = childNodes[9].Attributes["param"].Value;
			Vector3 target20 = Vector3.zero;
			if (value62 != "4294967295")
			{
				target20 = PositionParam(ObjectList[int.Parse(value62)].ChildNodes[0], RotatePos: false);
			}
			string value63 = childNodes[10].Attributes["param"].Value;
			Vector3 target21 = Vector3.zero;
			if (value63 != "4294967295")
			{
				target21 = PositionParam(ObjectList[int.Parse(value63)].ChildNodes[0], RotatePos: false);
			}
			string value64 = childNodes[11].Attributes["param"].Value;
			Vector3 target22 = Vector3.zero;
			if (value64 != "4294967295")
			{
				target22 = PositionParam(ObjectList[int.Parse(value64)].ChildNodes[0], RotatePos: false);
			}
			string value65 = childNodes[12].Attributes["param"].Value;
			Vector3 target23 = Vector3.zero;
			if (value65 != "4294967295")
			{
				target23 = PositionParam(ObjectList[int.Parse(value65)].ChildNodes[0], RotatePos: false);
			}
			gameObject.GetComponent<Dtd_BilliardCounter>().SetParameters(count2, target15, target16, target17, target18, target19, target20, target21, target22, target23);
		}
		if (Class == "dtd_switchcounter")
		{
			int count3 = int.Parse(childNodes[2].Attributes["param"].Value);
			string value66 = childNodes[3].Attributes["param"].Value;
			Vector3 target24 = Vector3.zero;
			if (value66 != "4294967295")
			{
				target24 = PositionParam(ObjectList[int.Parse(value66)].ChildNodes[0], RotatePos: false);
			}
			string value67 = childNodes[4].Attributes["param"].Value;
			Vector3 target25 = Vector3.zero;
			if (value67 != "4294967295")
			{
				target25 = PositionParam(ObjectList[int.Parse(value67)].ChildNodes[0], RotatePos: false);
			}
			string value68 = childNodes[5].Attributes["param"].Value;
			Vector3 target26 = Vector3.zero;
			if (value68 != "4294967295")
			{
				target26 = PositionParam(ObjectList[int.Parse(value68)].ChildNodes[0], RotatePos: false);
			}
			string value69 = childNodes[6].Attributes["param"].Value;
			Vector3 target27 = Vector3.zero;
			if (value69 != "4294967295")
			{
				target27 = PositionParam(ObjectList[int.Parse(value69)].ChildNodes[0], RotatePos: false);
			}
			string value70 = childNodes[7].Attributes["param"].Value;
			Vector3 target28 = Vector3.zero;
			if (value70 != "4294967295")
			{
				target28 = PositionParam(ObjectList[int.Parse(value70)].ChildNodes[0], RotatePos: false);
			}
			string value71 = childNodes[8].Attributes["param"].Value;
			Vector3 target29 = Vector3.zero;
			if (value71 != "4294967295")
			{
				target29 = PositionParam(ObjectList[int.Parse(value71)].ChildNodes[0], RotatePos: false);
			}
			string value72 = childNodes[9].Attributes["param"].Value;
			Vector3 target30 = Vector3.zero;
			if (value72 != "4294967295")
			{
				target30 = PositionParam(ObjectList[int.Parse(value72)].ChildNodes[0], RotatePos: false);
			}
			string value73 = childNodes[10].Attributes["param"].Value;
			Vector3 target31 = Vector3.zero;
			if (value73 != "4294967295")
			{
				target31 = PositionParam(ObjectList[int.Parse(value73)].ChildNodes[0], RotatePos: false);
			}
			string value74 = childNodes[11].Attributes["param"].Value;
			Vector3 target32 = Vector3.zero;
			if (value74 != "4294967295")
			{
				target32 = PositionParam(ObjectList[int.Parse(value74)].ChildNodes[0], RotatePos: false);
			}
			string value75 = childNodes[12].Attributes["param"].Value;
			gameObject.GetComponent<Dtd_SwitchCounter>().SetParameters(count3, target24, target25, target26, target27, target28, target29, target30, target31, target32, value75);
		}
		if (Class == "wap_pathsnowball")
		{
			string value76 = childNodes[2].Attributes["param"].Value;
			float speed13 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float distance3 = float.Parse(childNodes[4].Attributes["param"].Value);
			gameObject.GetComponent<PathSnowBall>().SetParameters(value76, speed13, distance3);
		}
		if (Class == "wap_brokensnowball")
		{
			float parameters10 = float.Parse(childNodes[3].Attributes["param"].Value);
			gameObject.GetComponent<BrokenSnowBall>().SetParameters(parameters10);
			gameObject.transform.position += gameObject.transform.up * 1.25f;
		}
		if (Class == "wap_searchlight")
		{
			float pitchAngle = float.Parse(childNodes[2].Attributes["param"].Value);
			float pitchRange = float.Parse(childNodes[3].Attributes["param"].Value);
			float pitchSpeed = float.Parse(childNodes[4].Attributes["param"].Value);
			float pitchTime = float.Parse(childNodes[5].Attributes["param"].Value);
			float yawAngle = float.Parse(childNodes[6].Attributes["param"].Value);
			float yawRange = float.Parse(childNodes[7].Attributes["param"].Value);
			float yawSpeed = float.Parse(childNodes[8].Attributes["param"].Value);
			float yawTime = float.Parse(childNodes[9].Attributes["param"].Value);
			float findTime = float.Parse(childNodes[10].Attributes["param"].Value);
			float loseLength = float.Parse(childNodes[11].Attributes["param"].Value) * Scale;
			string value77 = childNodes[12].Attributes["param"].Value;
			string value78 = childNodes[13].Attributes["param"].Value;
			bool allBrk = childNodes[14].Attributes["param"].Value == "1";
			gameObject.GetComponent<SearchLight>().SetParameters(pitchAngle, pitchRange, pitchSpeed, pitchTime, yawAngle, yawRange, yawSpeed, yawTime, findTime, loseLength, value77, value78, allBrk);
		}
		if (Class == "cscglassbuildbomb")
		{
			float parameters11 = float.Parse(childNodes[2].Attributes["param"].Value);
			gameObject.GetComponent<GlassBuild>().SetParameters(parameters11);
		}
		if (Class == "bldgexplode")
		{
			float parameters12 = float.Parse(childNodes[2].Attributes["param"].Value);
			gameObject.GetComponent<BldgExplode>().SetParameters(parameters12);
		}
		if (Class == "csctornadochase")
		{
			float parameters13 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			gameObject.GetComponent<TornadoChase>().SetParameters(parameters13);
		}
		if (Class == "ironspring")
		{
			string value79 = childNodes[2].Attributes["param"].Value;
			Vector3 target33 = Vector3.zero;
			if (value79 != "4294967295")
			{
				target33 = PositionParam(ObjectList[int.Parse(value79)].ChildNodes[0], RotatePos: false);
			}
			float pitch3 = float.Parse(childNodes[3].Attributes["param"].Value);
			float yaw2 = float.Parse(childNodes[4].Attributes["param"].Value);
			float power4 = float.Parse(childNodes[5].Attributes["param"].Value) * Scale;
			float out_Time2 = float.Parse(childNodes[6].Attributes["param"].Value);
			float on_Time2 = float.Parse(childNodes[7].Attributes["param"].Value);
			gameObject.GetComponent<IronSpring>().SetParameters(target33, pitch3, yaw2, power4, out_Time2, on_Time2);
		}
		if (Class == "crater")
		{
			int attackPower = int.Parse(childNodes[2].Attributes["param"].Value);
			float waitTime = float.Parse(childNodes[3].Attributes["param"].Value);
			float time7 = float.Parse(childNodes[4].Attributes["param"].Value);
			gameObject.GetComponent<Crater>().SetParameters(attackPower, waitTime, time7);
		}
		if (Class == "flamesingle")
		{
			float shineTime = float.Parse(childNodes[2].Attributes["param"].Value);
			bool eSPMode = childNodes[3].Attributes["param"].Value == "1";
			float light_R = float.Parse(childNodes[4].Attributes["param"].Value);
			float light_G = float.Parse(childNodes[5].Attributes["param"].Value);
			float light_B = float.Parse(childNodes[6].Attributes["param"].Value);
			float light_Range = float.Parse(childNodes[10].Attributes["param"].Value);
			string value80 = childNodes[11].Attributes["param"].Value;
			Vector3 signalObj = Vector3.zero;
			if (value80 != "4294967295")
			{
				signalObj = PositionParam(ObjectList[int.Parse(value80)].ChildNodes[0], RotatePos: false);
			}
			gameObject.GetComponent<FlameSingle>().SetParameters(shineTime, eSPMode, light_R, light_G, light_B, light_Range, signalObj);
		}
		if (Class == "flamesequence")
		{
			float prepareTime = float.Parse(childNodes[2].Attributes["param"].Value);
			string value81 = childNodes[3].Attributes["param"].Value;
			gameObject.GetComponent<FlameSequence>().SetParameters(prepareTime, value81);
		}
		if (Class == "inclinedstonebridge")
		{
			float time8 = float.Parse(childNodes[2].Attributes["param"].Value);
			float time9 = float.Parse(childNodes[3].Attributes["param"].Value);
			gameObject.GetComponent<InclinedBridge>().SetParameters(time8, time9);
		}
		if (Class == "flc_volcanicbomb")
		{
			float z13 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float x13 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float y13 = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			string value82 = childNodes[5].Attributes["param"].Value;
			Vector3 volcanoObj = Vector3.zero;
			if (value82 != "4294967295")
			{
				volcanoObj = PositionParam(ObjectList[int.Parse(value82)].ChildNodes[0], RotatePos: false);
			}
			float cycleTime = float.Parse(childNodes[6].Attributes["param"].Value);
			float volcanoTime = float.Parse(childNodes[10].Attributes["param"].Value);
			float bombHeight = float.Parse(childNodes[11].Attributes["param"].Value) * Scale;
			float bombRange = float.Parse(childNodes[12].Attributes["param"].Value) * Scale;
			BoxCollider component13 = gameObject.GetComponent<BoxCollider>();
			component13.size = new Vector3(x13, y13, z13);
			component13.center = new Vector3(0f, component13.size.y * 0.5f, 0f);
			gameObject.GetComponent<VolcanicBomb>().SetParameters(volcanoObj, cycleTime, volcanoTime, bombHeight, bombRange);
		}
		if (Class == "flc_flamecore")
		{
			float attackTime = float.Parse(childNodes[2].Attributes["param"].Value);
			float attackTime2 = float.Parse(childNodes[3].Attributes["param"].Value);
			float attackRange = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			int firstMode = int.Parse(childNodes[5].Attributes["param"].Value);
			string value83 = childNodes[6].Attributes["param"].Value;
			gameObject.GetComponent<FlameCore>().SetParameters(attackTime, attackTime2, attackRange, firstMode, value83);
		}
		if (Class == "rct_belt")
		{
			float forward = float.Parse(childNodes[2].Attributes["param"].Value);
			float backward = float.Parse(childNodes[3].Attributes["param"].Value);
			float time10 = float.Parse(childNodes[4].Attributes["param"].Value);
			gameObject.GetComponent<Belt>().SetParameters(forward, backward, time10);
		}
		if (Class == "rct_seesaw")
		{
			float height6 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float speed14 = float.Parse(childNodes[3].Attributes["param"].Value);
			float retSpeed = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			float time11 = float.Parse(childNodes[5].Attributes["param"].Value);
			float maxLen = float.Parse(childNodes[6].Attributes["param"].Value) * Scale;
			float difference = float.Parse(childNodes[7].Attributes["param"].Value) * Scale;
			gameObject.GetComponent<Seesaw>().SetParameters(height6, speed14, retSpeed, time11, maxLen, difference);
		}
		if (Class == "normal_train")
		{
			string value84 = childNodes[2].Attributes["param"].Value;
			float speed15 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float initialProgress = float.Parse(childNodes[4].Attributes["param"].Value);
			int num5 = int.Parse(childNodes[5].Attributes["param"].Value);
			bool loop = childNodes[7].Attributes["param"].Value == "1";
			Vector3 camera = VectorParam(childNodes[11], RotatePos: false);
			gameObject.GetComponent<Train>().SetParameters(value84, speed15, initialProgress, num5, loop, camera);
		}
		if (Class == "freight_train")
		{
			string value85 = childNodes[2].Attributes["param"].Value;
			float speed16 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float initialProgress2 = float.Parse(childNodes[4].Attributes["param"].Value);
			int num6 = int.Parse(childNodes[5].Attributes["param"].Value);
			int startHalfway = int.Parse(childNodes[7].Attributes["param"].Value);
			bool loop2 = childNodes[7].Attributes["param"].Value == "1";
			string value86 = childNodes[9].Attributes["param"].Value;
			string value87 = childNodes[10].Attributes["param"].Value;
			string value88 = childNodes[11].Attributes["param"].Value;
			string value89 = childNodes[12].Attributes["param"].Value;
			string value90 = childNodes[13].Attributes["param"].Value;
			string value91 = childNodes[14].Attributes["param"].Value;
			string value92 = childNodes[15].Attributes["param"].Value;
			string value93 = childNodes[16].Attributes["param"].Value;
			string value94 = childNodes[17].Attributes["param"].Value;
			string value95 = childNodes[18].Attributes["param"].Value;
			gameObject.GetComponent<Freight>().SetParameters(value85, speed16, initialProgress2, num6, startHalfway, loop2, value86, value87, value88, value89, value90, value91, value92, value93, value94, value95);
		}
		if (Class == "eggman_train")
		{
			string value96 = childNodes[2].Attributes["param"].Value;
			float speed17 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float initialProgress3 = float.Parse(childNodes[4].Attributes["param"].Value);
			int num7 = int.Parse(childNodes[5].Attributes["param"].Value);
			float interval2 = float.Parse(childNodes[6].Attributes["param"].Value) * Scale;
			bool loop3 = childNodes[7].Attributes["param"].Value == "1";
			int type6 = int.Parse(childNodes[9].Attributes["param"].Value);
			float bombTime = float.Parse(childNodes[10].Attributes["param"].Value);
			int hP = int.Parse(childNodes[11].Attributes["param"].Value);
			int hP2 = int.Parse(childNodes[12].Attributes["param"].Value);
			string value97 = childNodes[14].Attributes["param"].Value;
			string value98 = childNodes[15].Attributes["param"].Value;
			int @break = int.Parse(childNodes[16].Attributes["param"].Value);
			float waitTime2 = float.Parse(childNodes[17].Attributes["param"].Value);
			float distance4 = float.Parse(childNodes[18].Attributes["param"].Value) * Scale;
			bool left = childNodes[23].Attributes["param"].Value == "1";
			Vector3 camera2 = VectorParam(childNodes[24], RotatePos: false);
			gameObject.GetComponent<Eggtrain>().SetParameters(value96, speed17, initialProgress3, num7, interval2, loop3, type6, bombTime, hP, hP2, value97, value98, @break, waitTime2, distance4, left, camera2);
		}
		if (Class == "lotus")
		{
			float spd = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float spd2 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float spd3 = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			float charge = float.Parse(childNodes[5].Attributes["param"].Value);
			float charge2 = float.Parse(childNodes[6].Attributes["param"].Value);
			float charge3 = float.Parse(childNodes[7].Attributes["param"].Value);
			bool eSPMode2 = childNodes[8].Attributes["param"].Value == "1";
			float noConTime = float.Parse(childNodes[9].Attributes["param"].Value) / 2f;
			gameObject.GetComponent<Lotus>().SetParameters(spd, spd2, spd3, charge, charge2, charge3, eSPMode2, noConTime);
		}
		if (Class == "turtle")
		{
			float redownTime = float.Parse(childNodes[2].Attributes["param"].Value);
			float damageTime = float.Parse(childNodes[3].Attributes["param"].Value);
			float turnTime = float.Parse(childNodes[4].Attributes["param"].Value);
			float speedSwim = float.Parse(childNodes[5].Attributes["param"].Value) * Scale;
			bool doReverse = childNodes[6].Attributes["param"].Value == "1";
			bool doLoop = childNodes[7].Attributes["param"].Value == "1";
			bool waitFruit = childNodes[8].Attributes["param"].Value == "1";
			bool waitRide = childNodes[9].Attributes["param"].Value == "1";
			string value99 = childNodes[10].Attributes["param"].Value;
			gameObject.GetComponent<Turtle>().SetParameters(redownTime, damageTime, turnTime, speedSwim, doReverse, doLoop, waitFruit, waitRide, value99);
		}
		if (Class == "fruit")
		{
			string value100 = childNodes[2].Attributes["param"].Value;
			Vector3 parameters14 = Vector3.zero;
			if (value100 != "4294967295")
			{
				parameters14 = PositionParam(ObjectList[int.Parse(value100)].ChildNodes[0], RotatePos: false);
			}
			gameObject.GetComponent<Fruit>().SetParameters(parameters14);
		}
		if (Class == "bungee")
		{
			float spd4 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			string value101 = childNodes[4].Attributes["param"].Value;
			Vector3 targetObj = Vector3.zero;
			if (value101 != "4294967295")
			{
				targetObj = PositionParam(ObjectList[int.Parse(value101)].ChildNodes[0], RotatePos: false);
			}
			float motSpd = float.Parse(childNodes[5].Attributes["param"].Value);
			gameObject.GetComponent<Bungee>().SetParameters(spd4, targetObj, motSpd);
		}
		if (Class == "tarzan")
		{
			float top = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float bottom = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float jumpSpeed = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			float jumpCheckSpeed = float.Parse(childNodes[5].Attributes["param"].Value);
			bool fixDirection = childNodes[6].Attributes["param"].Value == "1";
			float hittutaSpeed = float.Parse(childNodes[7].Attributes["param"].Value);
			float weight = float.Parse(childNodes[8].Attributes["param"].Value);
			float hitAngle = float.Parse(childNodes[9].Attributes["param"].Value);
			float jumpCheckAngle = float.Parse(childNodes[10].Attributes["param"].Value);
			string value102 = childNodes[11].Attributes["param"].Value;
			Vector3 targetObj2 = Vector3.zero;
			if (value102 != "4294967295")
			{
				targetObj2 = PositionParam(ObjectList[int.Parse(value102)].ChildNodes[0], RotatePos: false);
			}
			float camZ = float.Parse(childNodes[12].Attributes["param"].Value);
			float camY = float.Parse(childNodes[13].Attributes["param"].Value);
			bool isRouge = childNodes[14].Attributes["param"].Value == "1";
			gameObject.GetComponent<Tarzan>().SetParameters(top, bottom, jumpSpeed, jumpCheckSpeed, fixDirection, hittutaSpeed, weight, hitAngle, jumpCheckAngle, targetObj2, camZ, camY, isRouge);
		}
		if (Class == "espswing")
		{
			float parameters15 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			gameObject.GetComponent<ESPSwing>().SetParameters(parameters15);
		}
		if (Class == "hangingrock")
		{
			bool parameters16 = childNodes[2].Attributes["param"].Value == "1";
			gameObject.GetComponent<HangingRock>().SetParameters(parameters16);
		}
		if (Class == "eagle")
		{
			float speed18 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			string value103 = childNodes[3].Attributes["param"].Value;
			bool lockPlayer = childNodes[4].Attributes["param"].Value == "1";
			gameObject.GetComponent<Eagle>().SetParameters(speed18, value103, lockPlayer);
		}
		if (Class == "rope")
		{
			Vector3 position3 = VectorParam(childNodes[2], RotatePos: false);
			float speedLow = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float speedMedium = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			float speedHigh = float.Parse(childNodes[5].Attributes["param"].Value) * Scale;
			float timeLow = float.Parse(childNodes[7].Attributes["param"].Value);
			float timeMedium = float.Parse(childNodes[8].Attributes["param"].Value);
			float timeHigh = float.Parse(childNodes[9].Attributes["param"].Value);
			gameObject.GetComponent<Rope>().SetParameters(position3, speedLow, speedMedium, speedHigh, timeLow, timeMedium, timeHigh);
		}
		if (Class == "inclinedbridge")
		{
			float time12 = float.Parse(childNodes[2].Attributes["param"].Value);
			float time13 = float.Parse(childNodes[3].Attributes["param"].Value);
			gameObject.GetComponentInChildren<InclinedBridge>().SetParameters(time12, time13);
		}
		if (Class == "windroad")
		{
			float appearTime = float.Parse(childNodes[3].Attributes["param"].Value);
			float disappearTime = float.Parse(childNodes[4].Attributes["param"].Value);
			string value104 = childNodes[5].Attributes["param"].Value;
			gameObject.GetComponent<WindRoad>().SetParameters(appearTime, disappearTime, value104);
		}
		if (Class == "brokenstairs_right" || Class == "brokenstairs_left")
		{
			float parameters17 = float.Parse(childNodes[2].Attributes["param"].Value);
			BrokenStairs[] componentsInChildren = gameObject.GetComponentsInChildren<BrokenStairs>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetParameters(parameters17);
			}
		}
		if (Class == "pendulum")
		{
			float parameters18 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			gameObject.GetComponentInChildren<Pendulum>().SetParameters(parameters18);
		}
		if (Class == "aqa_balancer")
		{
			float power5 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float deceleration = float.Parse(childNodes[3].Attributes["param"].Value);
			gameObject.GetComponent<Aqa_Balancer>().SetParameters(power5, deceleration);
		}
		if (Class == "aqa_magnet")
		{
			int kind = int.Parse(childNodes[2].Attributes["param"].Value);
			float radius5 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float force = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			float offTime2 = float.Parse(childNodes[5].Attributes["param"].Value);
			gameObject.GetComponent<Aqa_Magnet>().SetParameters(kind, radius5, force, offTime2);
		}
		if (Class == "aqa_mercury_small")
		{
			int hP3 = int.Parse(childNodes[2].Attributes["param"].Value);
			float friction = float.Parse(childNodes[3].Attributes["param"].Value);
			float air_Friction = float.Parse(childNodes[4].Attributes["param"].Value);
			gameObject.GetComponent<Aqa_Mercury_Small>().SetParameters(hP3, friction, air_Friction);
		}
		if (Class == "aqa_pond")
		{
			float length2 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float width = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float height7 = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			int num8 = int.Parse(childNodes[5].Attributes["param"].Value);
			int hP4 = int.Parse(childNodes[6].Attributes["param"].Value);
			float friction2 = float.Parse(childNodes[7].Attributes["param"].Value);
			float air_Friction2 = float.Parse(childNodes[8].Attributes["param"].Value);
			float vel = float.Parse(childNodes[9].Attributes["param"].Value);
			float time14 = float.Parse(childNodes[10].Attributes["param"].Value);
			gameObject.GetComponent<Aqa_Pond>().SetParameters(length2, width, height7, num8, hP4, friction2, air_Friction2, vel, time14);
		}
		if (Class == "aqa_launcher")
		{
			int num9 = int.Parse(childNodes[2].Attributes["param"].Value);
			int hP5 = int.Parse(childNodes[3].Attributes["param"].Value);
			float friction3 = float.Parse(childNodes[4].Attributes["param"].Value);
			float air_Friction3 = float.Parse(childNodes[5].Attributes["param"].Value);
			float speed19 = float.Parse(childNodes[6].Attributes["param"].Value) * Scale;
			float time15 = float.Parse(childNodes[7].Attributes["param"].Value);
			string value105 = childNodes[8].Attributes["param"].Value;
			Vector3 target34 = Vector3.zero;
			if (value105 != "4294967295")
			{
				target34 = PositionParam(ObjectList[int.Parse(value105)].ChildNodes[0], RotatePos: false);
			}
			gameObject.GetComponent<Aqa_Launcher>().SetParameters(num9, hP5, friction3, air_Friction3, speed19, time15, target34);
		}
		if (Class == "aqa_glass_blue" || Class == "aqa_glass_red")
		{
			float parameters19 = float.Parse(childNodes[2].Attributes["param"].Value);
			gameObject.GetComponent<Aqa_Glass>().SetParameters(parameters19);
		}
		if (Class == "townsgoal")
		{
			float z14 = float.Parse(childNodes[2].Attributes["param"].Value) * Scale;
			float x14 = float.Parse(childNodes[3].Attributes["param"].Value) * Scale;
			float y14 = float.Parse(childNodes[4].Attributes["param"].Value) * Scale;
			BoxCollider component14 = gameObject.GetComponent<BoxCollider>();
			component14.size = new Vector3(x14, y14, z14);
			component14.center = new Vector3(0f, component14.size.y * 0.5f, 0f);
		}
		if (Class == "warpgate")
		{
			string value106 = childNodes[2].Attributes["param"].Value;
			int appearances = int.Parse(childNodes[3].Attributes["param"].Value);
			gameObject.GetComponent<WarpGate>().SetParameters(value106, appearances);
		}
		return gameObject;
	}

	public static GameObject InstantiateResource(this string Path)
	{
		return UnityEngine.Object.Instantiate(Resources.Load<GameObject>(Path));
	}

	public static string GetEnemyString(string Container)
	{
		string text = Container.Split("_"[0])[0];
		StringReader stringReader = new StringReader(Container);
		stringReader.Skip(text.Length + 1);
		return stringReader.ReadToEnd();
	}

	public static Vector3 PositionParam(XmlNode PosParam, bool RotatePos = true)
	{
		float num = float.Parse(PosParam.Attributes["x"].Value);
		float y = float.Parse(PosParam.Attributes["y"].Value);
		float z = float.Parse(PosParam.Attributes["z"].Value);
		Vector3 vector = new Vector3(0f - num, y, z) * Scale;
		if (RotatePos)
		{
			vector = vector.RotateAround(Vector3.zero, Vector3.up, 270f);
		}
		return vector;
	}

	public static Vector3 VectorParam(XmlNode PositionParam, bool RotatePos = true)
	{
		float num = float.Parse(PositionParam.Attributes["param"].Value);
		float y = float.Parse(PositionParam.Attributes["param2"].Value);
		float z = float.Parse(PositionParam.Attributes["param3"].Value);
		Vector3 vector = new Vector3(0f - num, y, z) * Scale;
		if (RotatePos)
		{
			vector = vector.RotateAround(Vector3.zero, Vector3.up, 270f);
		}
		return vector;
	}

	public static Quaternion RotationParam(XmlNode RotationParam)
	{
		float z = float.Parse(RotationParam.Attributes["x"].Value);
		float num = float.Parse(RotationParam.Attributes["y"].Value);
		float x = float.Parse(RotationParam.Attributes["z"].Value);
		float w = float.Parse(RotationParam.Attributes["w"].Value);
		return new Quaternion(x, 0f - num, z, w) * Quaternion.Euler(0f, 270f, 0f);
	}
}
