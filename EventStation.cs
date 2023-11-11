using UnityEngine;
using UnityEngine.SceneManagement;

public class EventStation : ObjectBase
{
	internal void CallEvent(string EventName, GameObject Transmitter = null)
	{
		string text = SceneManager.GetActiveScene().name;
		string stage = text.Split('_')[0];
		string text2 = text.Split('_')[1];
		string text3 = text.Split('_')[2];
		SetFunction(stage, text2 + "_" + text3, EventName, Transmitter);
	}

	private void SetFunction(string Stage, string CharSection, string Function, GameObject Transmitter = null)
	{
		if (Stage == "test")
		{
			if (CharSection == "a_sn")
			{
				if (Function == "goto_b")
				{
					Game.ChangeArea("test_b_sn");
				}
				if (Function == "goto_c")
				{
					Game.ChangeArea("test_c_sn");
				}
				if (Function == "test_cage")
				{
					Game.Signal("common_cage00");
				}
				if (Function == "test_cage2")
				{
					Game.Signal("common_cage01");
				}
				if (Function == "test_switch")
				{
					Game.ProcessMessage("common_laser00", "GateOpen");
					Game.ProcessMessage("common_laser01", "GateOpen");
					Game.ProcessMessage("common_laser02", "GateOpen");
					Game.ProcessMessage("common_laser03", "GateOpen");
					Game.ProcessMessage("common_laser04", "GateOpen");
					Game.ProcessMessage("common_laser05", "GateOpen");
					Game.ProcessMessage("common_laser06", "GateOpen");
					Game.ProcessMessage("common_laser07", "GateOpen");
				}
			}
			if (CharSection == "b_sn")
			{
				if (Function == "goto_kdv")
				{
					Game.ChangeArea("kdv_e_sn", Game.MenuLoadMode);
				}
				if (Function == "goto_csc")
				{
					Game.ChangeArea("csc_f_sv", Game.MenuLoadMode);
				}
			}
			if (CharSection == "c_sn")
			{
				if (Function == "cage1000")
				{
					Game.Signal("common_cage1000");
				}
				if (Function == "cage1001")
				{
					Game.Signal("common_cage1001");
				}
				if (Function == "goto_b")
				{
					Game.ChangeArea("test_b_sn");
				}
				if (Function == "open_laser")
				{
					Game.ProcessMessage("common_laser1000", "GateOpen");
					Game.ProcessMessage("common_laser1001", "GateOpen");
					Game.ProcessMessage("common_laser1002", "GateOpen");
					Game.ProcessMessage("common_laser1003", "GateOpen");
					Game.ProcessMessage("common_laser1004", "GateOpen");
					Game.ProcessMessage("common_laser1005", "GateOpen");
					Game.ProcessMessage("common_laser1006", "GateOpen");
					Game.ProcessMessage("common_laser1007", "GateOpen");
					Game.StartEntityByName("GroupHelper1000");
				}
			}
		}
		if (Stage == "wvo")
		{
			if (CharSection == "a_sn")
			{
				if (Function == "goto_b")
				{
					Game.ChangeArea("wvo_b_sn");
				}
				if (Function == "brake01")
				{
					Game.Signal("objectphysics154");
				}
				if (Function == "orca01")
				{
					Game.Signal("wvo_orca01");
				}
				if (Function == "orca02")
				{
					Game.Signal("wvo_orca03");
				}
				if (Function == "orca03")
				{
					Game.Signal("wvo_orca04");
				}
				if (Function == "door01")
				{
					Game.ProcessMessage("wvo_doorB01", "GateClose");
					Game.StartEntityByName("GroupHelper11");
					Game.ProcessMessage("wvo_orca02", "GateClose");
				}
				if (Function == "move")
				{
					Game.ProcessMessage("wvo_battleship_custom", "Shoot", Transmitter);
				}
			}
			if (CharSection == "a_tl")
			{
				if (Function == "orca01")
				{
					Game.Signal("wvo_orca01");
				}
				if (Function == "orca03")
				{
					Game.Signal("wvo_orca03");
				}
				if (Function == "orca04")
				{
					Game.Signal("wvo_orca04");
				}
				if (Function == "orca05")
				{
					Game.Signal("wvo_orca05");
				}
				if (Function == "orca06")
				{
					Game.Signal("wvo_orca06");
				}
				if (Function == "carrier100")
				{
					Game.Signal("objdestroyer00");
					Game.StartEntityByName("Carrier00");
				}
				if (Function == "carrier101")
				{
					Game.Signal("objdestroyer01");
					Game.StartEntityByName("Carrier01");
				}
				if (Function == "carrier102")
				{
					Game.Signal("objdestroyer02");
					Game.StartEntityByName("Carrier02");
				}
				if (Function == "carrier103")
				{
					Game.Signal("objdestroyer03");
					Game.StartEntityByName("Carrier03");
				}
				if (Function == "carrier104")
				{
					Game.Signal("objdestroyer04");
					Game.StartEntityByName("Carrier04");
				}
				if (Function == "move100")
				{
					Game.ProcessMessage("wvo_battleship100", "Shoot", Transmitter);
				}
				if (Function == "move101")
				{
					Game.ProcessMessage("wvo_battleship101", "Shoot", Transmitter);
				}
				if (Function == "move102")
				{
					Game.ProcessMessage("wvo_battleship102", "Shoot", Transmitter);
				}
				if (Function == "move103")
				{
					Game.ProcessMessage("wvo_battleship103", "Shoot", Transmitter);
				}
				if (Function == "move104")
				{
					Game.ProcessMessage("wvo_battleship104", "Shoot", Transmitter);
				}
			}
			if (CharSection == "a_sd")
			{
				if (Function == "open_cage")
				{
					Game.Signal("common_cage01");
				}
				if (Function == "orca01")
				{
					Game.Signal("wvo_orca04");
				}
				if (Function == "orca02")
				{
					Game.Signal("wvo_orca03");
				}
				if (Function == "open_cage02")
				{
					Game.Signal("common_cage03");
				}
				if (Function == "open_cage03")
				{
					Game.Signal("common_cage02");
				}
				if (Function == "door01")
				{
					Game.Signal("wvo_doorA02");
					Game.Signal("common_hintcollision_destroy");
					Game.StartEntityByName("DoorGroup");
				}
			}
			if (CharSection == "a_bz")
			{
				if (Function == "orca01")
				{
					Game.Signal("wvo_orca01");
				}
				if (Function == "orca03")
				{
					Game.Signal("wvo_orca03");
				}
				if (Function == "orca04")
				{
					Game.Signal("wvo_orca04");
				}
				if (Function == "orca05")
				{
					Game.Signal("wvo_orca05");
				}
				if (Function == "orca06")
				{
					Game.Signal("wvo_orca06");
				}
				if (Function == "common_cage1000")
				{
					Game.Signal("common_cage1000");
				}
				if (Function == "cage1001")
				{
					Game.Signal("common_cage1001");
				}
			}
			if (CharSection == "b_sn" && Function == "move")
			{
				Game.ProcessMessage("wvo_battleship01", "Shoot", Transmitter);
			}
			if (CharSection == "b_sd")
			{
				if (Function == "goto_a")
				{
					Game.ChangeArea("wvo_a_sd");
				}
				if (Function == "brake01")
				{
					Game.Signal("objectphysics02");
				}
				if (Function == "brake02")
				{
					Game.Signal("objectphysics35");
				}
				if (Function == "brake03")
				{
					Game.Signal("objectphysics49");
				}
				if (Function == "brake04")
				{
					Game.Signal("objectphysics50");
				}
				if (Function == "brake05")
				{
					Game.Signal("objectphysics06");
				}
			}
		}
		if (Stage == "dtd")
		{
			if (CharSection == "a_sn")
			{
				if (Function == "pillar01")
				{
					Game.Signal("dtd_pillar01");
					Game.Signal("dtd_pillar02");
					Game.Signal("dtd_pillar03");
					Game.Signal("dtd_pillar12");
				}
				if (Function == "cage01")
				{
					Game.Signal("common_cage01");
				}
				if (Function == "pillar03")
				{
					Game.Signal("dtd_pillar05");
					Game.Signal("dtd_pillar04");
				}
				if (Function == "pillar04")
				{
					Game.Signal("dtd_pillar13");
					Game.Signal("dtd_pillar14");
				}
			}
			if (CharSection == "a_sd")
			{
				if (Function == "pillare01")
				{
					Game.Signal("dtd_pillar_eagle29");
					Game.Signal("dtd_pillar_eagle30");
				}
				if (Function == "pillare02")
				{
					Game.Signal("dtd_pillar_eagle31");
					Game.Signal("dtd_pillar_eagle32");
				}
				if (Function == "pillare03")
				{
					Game.Signal("dtd_pillar_eagle33");
					Game.Signal("dtd_pillar_eagle34");
				}
				if (Function == "pillare04")
				{
					Game.Signal("dtd_pillar_eagle07");
					Game.Signal("dtd_pillar_eagle08");
				}
				if (Function == "pillare05")
				{
					Game.Signal("dtd_pillar_eagle11");
					Game.Signal("dtd_pillar_eagle12");
				}
				if (Function == "pillare06")
				{
					Game.Signal("dtd_pillar_eagle35");
					Game.Signal("dtd_pillar_eagle36");
				}
				if (Function == "pillare15")
				{
					Game.Signal("dtd_pillar_eagle13");
					Game.Signal("dtd_pillar_eagle14");
				}
				if (Function == "pillare07")
				{
					Game.Signal("dtd_pillar_eagle43");
					Game.Signal("dtd_pillar_eagle44");
				}
				if (Function == "pillare17")
				{
					Game.Signal("dtd_pillar_eagle16");
					Game.Signal("dtd_pillar_eagle15");
				}
				if (Function == "pillare08")
				{
					Game.Signal("dtd_pillar_eagle17");
					Game.Signal("dtd_pillar_eagle18");
				}
				if (Function == "pillare09")
				{
					Game.Signal("dtd_pillar_eagle19");
					Game.Signal("dtd_pillar_eagle20");
				}
				if (Function == "pillare10")
				{
					Game.Signal("dtd_pillar_eagle23");
					Game.Signal("dtd_pillar_eagle24");
				}
				if (Function == "pillare11")
				{
					Game.Signal("dtd_pillar_eagle25");
					Game.Signal("dtd_pillar_eagle26");
				}
				if (Function == "pillare12")
				{
					Game.Signal("dtd_pillar_eagle27");
					Game.Signal("dtd_pillar_eagle28");
				}
				if (Function == "pillare30")
				{
					Game.Signal("dtd_pillar_eagle41");
					Game.Signal("dtd_pillar_eagle42");
				}
				if (Function == "pillare13")
				{
					Game.Signal("dtd_pillar_eagle39");
					Game.Signal("dtd_pillar_eagle40");
				}
				if (Function == "pillare14")
				{
					Game.Signal("dtd_pillar_eagle37");
					Game.Signal("dtd_pillar_eagle38");
				}
				if (Function == "door01")
				{
					Game.Signal("dtd_door01");
				}
				if (Function == "goto_b")
				{
					Game.ChangeArea("dtd_b_sd");
				}
			}
			if (CharSection == "b_sd")
			{
				if (Function == "door01")
				{
					Game.Signal("dtd_door01");
				}
				if (Function == "door02")
				{
					Game.Signal("dtd_door02");
				}
				if (Function == "door03")
				{
					Game.Signal("dtd_door04");
				}
				if (Function == "door04")
				{
					Game.Signal("dtd_door08");
				}
				if (Function == "door05")
				{
					Game.Signal("dtd_door09");
					Game.StartEntityByName("dialogue_group");
				}
				if (Function == "door06")
				{
					Game.Signal("dtd_door10");
				}
				if (Function == "door07")
				{
					Game.Signal("dtd_pillar03");
					Game.Signal("dtd_pillar04");
					Game.Signal("dtd_door06");
				}
				if (Function == "door08")
				{
					Game.Signal("dtd_door05");
				}
				if (Function == "door09")
				{
					Game.Signal("dtd_door12");
				}
				if (Function == "door10")
				{
					Game.StartEntityByName("amigo_collision01");
					Game.Signal("dtd_door03");
				}
				if (Function == "pillar01")
				{
					Game.Signal("dtd_pillar01");
					Game.Signal("dtd_pillar02");
				}
				if (Function == "pillar02")
				{
					Game.Signal("dtd_pillar05");
					Game.Signal("dtd_pillar06");
					Game.Signal("dtd_pillar07");
				}
				if (Function == "door12")
				{
					Game.Signal("dtd_door14");
				}
				if (Function == "door_close/dtd_door03")
				{
					Game.ProcessMessage("dtd_door03", "GateClose");
				}
				if (Function == "door1001")
				{
					Game.Signal("dtd_door11");
				}
				if (Function == "sharddoor")
				{
					Game.Signal("dtd_door07");
				}
				if (Function == "cage1000")
				{
					Game.Signal("common_cage1000");
				}
				if (Function == "door1002")
				{
					Game.Signal("dtd_door1000");
				}
				if (Function == "omega_door")
				{
					Game.StartEntityByName("OmegaDoor");
				}
			}
			if (CharSection == "b_sv")
			{
				if (Function == "door01")
				{
					Game.Signal("dtd_door01");
					Game.StartEntityByName("common_hint_collision1001");
				}
				if (Function == "door02")
				{
					Game.Signal("dtd_door06");
				}
				if (Function == "pillar01")
				{
					Game.Signal("dtd_pillar01");
					Game.Signal("dtd_pillar02");
					Game.Signal("dtd_pillar03");
				}
				if (Function == "pillar02")
				{
					Game.StartEntityByName("amigo_collision01");
					Game.ProcessMessage("dtd_door12", "GateClose");
					Game.Signal("dtd_door1000");
				}
				if (Function == "door17")
				{
					Game.Signal("dtd_door10");
					Game.StartEntityByName("billiard_1st_finished");
				}
				if (Function == "door03")
				{
					Game.Signal("dtd_door11");
				}
				if (Function == "door04")
				{
					Game.Signal("dtd_door12");
				}
				if (Function == "door05")
				{
					Game.Signal("dtd_door13");
				}
				if (Function == "door06")
				{
					Game.Signal("dtd_door14");
				}
				if (Function == "door07")
				{
					Game.Signal("dtd_door15");
				}
				if (Function == "door08")
				{
					Game.Signal("dtd_door16");
					Game.StartEntityByName("door16");
				}
				if (Function == "door09")
				{
					Game.Signal("dtd_door17");
				}
				if (Function == "door10")
				{
					Game.Signal("dtd_door18");
				}
				if (Function == "door12")
				{
					Game.Signal("dtd_door03");
				}
				if (Function == "door13")
				{
					Game.Signal("dtd_door05");
					Game.ProcessMessage("dtd_door05", "PsiEffect", true);
				}
				if (Function == "door14")
				{
					Game.Signal("dtd_door19");
					Game.StartEntityByName("common_hint_collision07");
				}
				if (Function == "door15")
				{
					Game.Signal("dtd_door20");
				}
				if (Function == "door16")
				{
					Game.Signal("dtd_door04");
					Game.ProcessMessage("dtd_door04", "PsiEffect", true);
				}
				if (Function == "door18")
				{
					Game.Signal("dtd_door09");
				}
				if (Function == "signal/dtd_door08")
				{
					Game.ProcessMessage("dtd_door09", "GateClose");
				}
				if (Function == "door_close/dtd_door06")
				{
					Game.ProcessMessage("dtd_door06", "GateClose");
				}
				if (Function == "pillar02_custom")
				{
					Game.ProcessMessage("dtd_door1000", "GateClose");
					Game.StartEntityByName("Door1000");
				}
				if (Function == "door_collision07")
				{
					Game.ProcessMessage("dtd_door19", "GateClose");
					Game.StartEntityByName("door_collision07");
				}
				if (Function == "lotusup_door01")
				{
					Game.StartEntityByName("UpgradeGroupHelper1000");
				}
				if (Function == "new_door07")
				{
					Game.Signal("dtd_door07");
				}
				if (Function == "lotusup_ladder01")
				{
					Game.Signal("physicspath1000");
					Game.ProcessMessage("physicspath1000", "PsiEffect", true);
					Game.Signal("physicspath1001");
					Game.ProcessMessage("physicspath1001", "PsiEffect", true);
					Game.Signal("physicspath1002");
					Game.ProcessMessage("physicspath1002", "PsiEffect", true);
					Game.Signal("physicspath1003");
					Game.ProcessMessage("physicspath1003", "PsiEffect", true);
					Game.Signal("physicspath1004");
					Game.ProcessMessage("physicspath1004", "PsiEffect", true);
					Game.Signal("physicspath1005");
					Game.ProcessMessage("physicspath1005", "PsiEffect", true);
				}
				if (Function == "lotusup_ladder02")
				{
					Game.Signal("physicspath1010");
					Game.ProcessMessage("physicspath1000", "PsiEffect", true);
					Game.Signal("physicspath1011");
					Game.ProcessMessage("physicspath1001", "PsiEffect", true);
					Game.Signal("physicspath1012");
					Game.ProcessMessage("physicspath1002", "PsiEffect", true);
					Game.Signal("physicspath1013");
					Game.ProcessMessage("physicspath1003", "PsiEffect", true);
					Game.Signal("physicspath1014");
					Game.ProcessMessage("physicspath1004", "PsiEffect", true);
					Game.Signal("physicspath1015");
					Game.ProcessMessage("physicspath1005", "PsiEffect", true);
				}
				if (Function == "lotusup_cage01")
				{
					Game.Signal("common_cage1000");
				}
			}
		}
		if (Stage == "wap")
		{
			if (CharSection == "a_sn")
			{
				if (Function == "yuki")
				{
					Game.ProcessMessage("wap_pathsnowball01", "Shoot", Transmitter);
				}
				if (Function == "goto_b")
				{
					Game.ChangeArea("wap_b_sn");
				}
				if (Function == "door02")
				{
					Game.Signal("wap_door01");
				}
				if (Function == "door100")
				{
					Game.Signal("wap_door100");
				}
			}
			if (CharSection == "a_sd")
			{
				if (Function == "goto_b")
				{
					Game.ChangeArea("wap_b_sd");
				}
				if (Function == "door01")
				{
					Game.Signal("wap_door01");
					Game.StartEntityByName("GroupHelper1000");
				}
				if (Function == "laser01")
				{
					Game.ProcessMessage("common_laser01", "GateOpen");
					Game.ProcessMessage("common_laser02", "GateOpen");
				}
				if (Function == "enemy01")
				{
					Game.StartEntityByName("GroupHelper10");
				}
				if (Function == "door01_end")
				{
					Game.StartEntityByName("GroupHelper1001");
				}
			}
			if (CharSection == "a_sv")
			{
				if (Function == "yuki")
				{
					Game.ProcessMessage("wap_pathsnowball01", "Shoot", Transmitter);
				}
				if (Function == "goto_b")
				{
					Game.ChangeArea("wap_b_sv");
				}
				if (Function == "laser01")
				{
					Game.ProcessMessage("common_laser21", "GateOpen");
					Game.ProcessMessage("common_laser22", "GateOpen");
				}
				if (Function == "door02")
				{
					Game.Signal("wap_door04");
					Game.StartEntityByName("DoorCutscene");
				}
			}
			if (CharSection == "b_sn")
			{
				if (Function == "laser01")
				{
					Game.ProcessMessage("common_laser25", "GateOpen");
					Game.ProcessMessage("common_laser26", "GateOpen");
				}
				if (Function == "enemy01")
				{
					Game.StartEntityByName("GroupHelper01");
					Game.StartEntityByName("HintCollisionGroup");
				}
				if (Function == "enemy02")
				{
					Game.StartEntityByName("GroupHelper02");
					Game.ProcessMessage("common_laser32", "GateClose");
					Game.ProcessMessage("common_laser31", "GateClose");
				}
				if (Function == "enemy03")
				{
					Game.ProcessMessage("common_laser33", "GateClose");
					Game.ProcessMessage("common_laser34", "GateClose");
					Game.StartEntityByName("GroupHelper03");
				}
				if (Function == "laser03")
				{
					Game.ProcessMessage("common_laser33", "GateOpen");
					Game.ProcessMessage("common_laser34", "GateOpen");
				}
				if (Function == "enemy04")
				{
					Game.StartEntityByName("GroupHelper08");
				}
				if (Function == "laser04")
				{
					Game.ProcessMessage("common_laser35", "GateOpen");
					Game.ProcessMessage("common_laser36", "GateOpen");
				}
				if (Function == "enemy05")
				{
					Game.StartEntityByName("GroupHelper04");
				}
				if (Function == "door01")
				{
					Game.ProcessMessage("wap_door03", "GateOpen");
					Game.Signal("wap_door01");
					Game.StartEntityByName("GroupHelper100");
					Game.ProcessMessage("common_laser35", "GateClose");
					Game.ProcessMessage("common_laser36", "GateClose");
					Game.ProcessMessage("common_laser2000000", "GateClose");
					Game.ProcessMessage("common_laser2000001", "GateClose");
				}
				if (Function == "door_close/wap_door03")
				{
					Game.ProcessMessage("wap_door03", "GateClose");
				}
				if (Function == "laser06")
				{
					Game.ProcessMessage("common_laser31", "GateOpen");
					Game.ProcessMessage("common_laser32", "GateOpen");
				}
				if (Function == "door02")
				{
					Game.ProcessMessage("wap_door06", "GateOpen");
				}
				if (Function == "door_close/wap_door06")
				{
					Game.ProcessMessage("wap_door06", "GateClose");
				}
				if (Function == "switch01")
				{
					Game.ProcessMessage("common_laser11", "GateOpen");
					Game.ProcessMessage("common_laser12", "GateOpen");
				}
				if (Function == "enemy10")
				{
					Game.StartEntityByName("GroupHelper14");
					Game.StartEntityByName("HintCollisionGroup");
				}
				if (Function == "enemy11")
				{
					Game.StartEntityByName("GroupHelper15");
					Game.StartEntityByName("HintCollisionGroup");
				}
				if (Function == "enemy12")
				{
					Game.StartEntityByName("GroupHelper16");
					Game.StartEntityByName("HintCollisionGroup");
				}
				if (Function == "door101")
				{
					Game.Signal("wap_door02");
				}
				if (Function == "caught100")
				{
					Game.StartEntityByName("HintCollisionGroup");
				}
			}
			if (CharSection == "b_sd")
			{
				if (Function == "enemy01")
				{
					Game.StartEntityByName("GroupHelper01");
				}
				if (Function == "enemy02")
				{
					Game.StartEntityByName("GroupHelper03");
				}
				if (Function == "enemy03")
				{
					Game.StartEntityByName("GroupHelper04");
				}
				if (Function == "enemy04")
				{
					Game.StartEntityByName("GroupHelper05");
				}
				if (Function == "laser03")
				{
					Game.ProcessMessage("common_laser11", "GateOpen");
					Game.ProcessMessage("common_laser12", "GateOpen");
				}
				if (Function == "laser04")
				{
					Game.ProcessMessage("common_laser07", "GateClose");
					Game.ProcessMessage("common_laser08", "GateClose");
					Game.ProcessMessage("common_laser13", "GateClose");
					Game.ProcessMessage("common_laser14", "GateClose");
				}
				if (Function == "laser05")
				{
					Game.ProcessMessage("common_laser07", "GateOpen");
					Game.ProcessMessage("common_laser08", "GateOpen");
					Game.ProcessMessage("common_laser13", "GateOpen");
					Game.ProcessMessage("common_laser14", "GateOpen");
				}
				if (Function == "enemy11")
				{
					Game.StartEntityByName("GroupHelper20");
				}
				if (Function == "enemy12")
				{
					Game.StartEntityByName("GroupHelper21");
				}
				if (Function == "enemy13")
				{
					Game.StartEntityByName("GroupHelper22");
				}
				if (Function == "enemy14")
				{
					Game.StartEntityByName("GroupHelper25");
				}
				if (Function == "door01")
				{
					Game.Signal("wap_door03");
					Game.StartEntityByName("common_hint_collision15");
				}
				if (Function == "enemy15")
				{
					Game.StartEntityByName("GroupHelper15");
				}
				if (Function == "ring01")
				{
					Game.StartEntityByName("GroupHelper19");
				}
				if (Function == "door_close/wap_door03")
				{
					Game.ProcessMessage("wap_door03", "GateClose");
				}
				if (Function == "wap_all_destroy")
				{
					Game.Result();
				}
				if (Function == "door05")
				{
					Game.ProcessMessage("wap_door05", "GateOpen");
				}
				if (Function == "laser1000")
				{
					Game.ProcessMessage("common_laser1000", "GateOpen");
					Game.ProcessMessage("common_laser1001", "GateOpen");
					Game.ProcessMessage("common_laser1002", "GateOpen");
					Game.ProcessMessage("common_laser1003", "GateOpen");
				}
			}
			if (CharSection == "b_sv")
			{
				if (Function == "enemy01")
				{
					Game.StartEntityByName("GroupHelper01");
				}
				if (Function == "door01")
				{
					Game.ProcessMessage("wap_door03", "GateOpen");
				}
				if (Function == "enemy02")
				{
					Game.StartEntityByName("GroupHelper03");
				}
				if (Function == "enemy03")
				{
					Game.StartEntityByName("GroupHelper04");
				}
				if (Function == "enemy04")
				{
					Game.StartEntityByName("GroupHelper05");
				}
				if (Function == "esp01")
				{
					Game.Signal("physicspath01");
					Game.ProcessMessage("physicspath01", "PsiEffect", true);
					Game.Signal("physicspath02");
					Game.ProcessMessage("physicspath02", "PsiEffect", true);
				}
				if (Function == "cage01")
				{
					Game.Signal("common_cage01");
				}
				if (Function == "hakai01")
				{
					Game.ProcessMessage("common_laser52", "GateOpen");
					Game.ProcessMessage("common_laser53", "GateOpen");
				}
				if (Function == "enemy100")
				{
					Game.StartEntityByName("GroupHelper26");
				}
				if (Function == "door1000")
				{
					Game.StartEntityByName("door04_col");
					Game.ProcessMessage("wap_door04", "GateClose");
				}
				if (Function == "espmark1000")
				{
					Game.Signal("objectphysics04");
				}
				if (Function == "espmark1001")
				{
					Game.Signal("objectphysics170");
				}
				if (Function == "espmark1002")
				{
					Game.Signal("objectphysics02");
				}
			}
		}
		if (Stage == "csc")
		{
			if (CharSection == "a_sn")
			{
				if (Function == "AtoB")
				{
					Game.ChangeArea("csc_b_sn");
				}
				if (Function == "explosion_001")
				{
					Game.Signal("bldgexplode01");
					Game.Signal("bldgexplode02");
					Game.Signal("bldgexplode03");
					Game.Signal("bldgexplode11");
					Game.Signal("cscglassbuildbomb01");
					Game.Signal("cscglassbuildbomb02");
					Game.Signal("cscglassbuildbomb03");
					Game.Signal("cscglassbuildbomb11");
				}
				if (Function == "explosion_002")
				{
					Game.Signal("bldgexplode04");
					Game.Signal("bldgexplode05");
					Game.Signal("cscglassbuildbomb04");
					Game.Signal("cscglassbuildbomb05");
				}
				if (Function == "explosion_003")
				{
					Game.Signal("bldgexplode06");
					Game.Signal("bldgexplode07");
					Game.Signal("bldgexplode08");
					Game.Signal("cscglassbuildbomb06");
					Game.Signal("cscglassbuildbomb07");
					Game.Signal("cscglassbuildbomb08");
				}
				if (Function == "explosion_004")
				{
					Game.Signal("bldgexplode09");
					Game.Signal("bldgexplode10");
					Game.Signal("cscglassbuildbomb09");
					Game.Signal("cscglassbuildbomb10");
				}
				if (Function == "explosion_005")
				{
					Game.Signal("bldgexplode12");
					Game.Signal("bldgexplode15");
					Game.Signal("bldgexplode16");
					Game.Signal("cscglassbuildbomb12");
					Game.Signal("cscglassbuildbomb15");
					Game.Signal("cscglassbuildbomb16");
				}
				if (Function == "explosion_006")
				{
					Game.Signal("bldgexplode17");
					Game.Signal("bldgexplode18");
					Game.Signal("cscglassbuildbomb17");
					Game.Signal("cscglassbuildbomb18");
				}
				if (Function == "explosion_007")
				{
					Game.Signal("bldgexplode19");
					Game.Signal("bldgexplode20");
					Game.Signal("cscglassbuildbomb19");
					Game.Signal("cscglassbuildbomb20");
				}
				if (Function == "glassbreak001")
				{
					Game.Signal("objectphysics44");
					Game.Signal("objectphysics45");
					Game.Signal("objectphysics46");
					Game.Signal("objectphysics47");
					Game.Signal("objectphysics48");
					Game.Signal("objectphysics49");
					Game.Signal("objectphysics50");
					Game.Signal("impulsesphere01");
					Game.Signal("bldgexplode13");
				}
				if (Function == "glassbreak002")
				{
					Game.Signal("objectphysics51");
					Game.Signal("objectphysics52");
					Game.Signal("objectphysics53");
					Game.Signal("objectphysics54");
					Game.Signal("objectphysics55");
					Game.Signal("impulsesphere02");
					Game.Signal("bldgexplode14");
				}
				if (Function == "signal/impulsesphere03")
				{
					Game.Signal("impulsesphere03");
				}
			}
			if (CharSection == "a_sd")
			{
				if (Function == "AtoB")
				{
					Game.ChangeArea("csc_b_sd");
				}
				if (Function == "explosion_001")
				{
					Game.Signal("bldgexplode00");
					Game.Signal("bldgexplode01");
					Game.Signal("bldgexplode02");
					Game.Signal("cscglassbuildbomb00");
					Game.Signal("cscglassbuildbomb01");
					Game.Signal("cscglassbuildbomb02");
				}
				if (Function == "explosion_002")
				{
					Game.Signal("bldgexplode03");
					Game.Signal("bldgexplode04");
					Game.Signal("bldgexplode05");
					Game.Signal("cscglassbuildbomb03");
					Game.Signal("cscglassbuildbomb04");
					Game.Signal("cscglassbuildbomb05");
				}
				if (Function == "explosion_003")
				{
					Game.Signal("bldgexplode06");
					Game.Signal("bldgexplode07");
					Game.Signal("cscglassbuildbomb06");
					Game.Signal("cscglassbuildbomb07");
				}
				if (Function == "explosion_008")
				{
					Game.Signal("bldgexplode08");
					Game.Signal("cscglassbuildbomb08");
				}
				if (Function == "explosion_005")
				{
					Game.Signal("bldgexplode15");
					Game.Signal("cscglassbuildbomb15");
				}
				if (Function == "explosion_006")
				{
					Game.Signal("bldgexplode16");
					Game.Signal("bldgexplode17");
					Game.Signal("cscglassbuildbomb16");
					Game.Signal("cscglassbuildbomb17");
				}
				if (Function == "explosion_007")
				{
					Game.Signal("bldgexplode18");
					Game.Signal("bldgexplode19");
					Game.Signal("cscglassbuildbomb18");
					Game.Signal("cscglassbuildbomb19");
				}
			}
			if (CharSection == "b_sn")
			{
				if (Function == "glassbreak001")
				{
					Game.Signal("physicsglass01");
					Game.Signal("impulsesphere201");
				}
				if (Function == "glassbreak002")
				{
					Game.Signal("physicsglass02");
					Game.Signal("impulsesphere202");
				}
				if (Function == "glassbreak003")
				{
					Game.Signal("physicsglass03");
					Game.Signal("impulsesphere203");
				}
				if (Function == "glassbreak004")
				{
					Game.Signal("physicsglass04");
					Game.Signal("impulsesphere204");
				}
				if (Function == "glassbreak005")
				{
					Game.Signal("physicsglass05");
					Game.Signal("impulsesphere205");
				}
				if (Function == "glassbreak006")
				{
					Game.Signal("physicsglass06");
					Game.Signal("impulsesphere206");
				}
				if (Function == "glassbreak007")
				{
					Game.Signal("physicsglass07");
					Game.Signal("impulsesphere207");
				}
				if (Function == "glassbreak008")
				{
					Game.Signal("physicsglass08");
					Game.Signal("impulsesphere208");
				}
				if (Function == "glassbreak009")
				{
					Game.Signal("physicsglass09");
					Game.Signal("impulsesphere209");
				}
				if (Function == "glassbreak010")
				{
					Game.Signal("physicsglass10");
					Game.Signal("impulsesphere210");
				}
				if (Function == "glassbreak011")
				{
					Game.Signal("physicsglass11");
					Game.Signal("impulsesphere211");
				}
				if (Function == "glassbreak012")
				{
					Game.Signal("physicsglass12");
					Game.Signal("impulsesphere212");
				}
				if (Function == "explosion_001")
				{
					Game.Signal("bldgexplode01");
					Game.Signal("Physicsglass38");
					Game.Signal("Physicsglass39");
					Game.Signal("Physicsglass40");
					Game.Signal("Physicsglass42");
					Game.Signal("Physicsglass43");
					Game.Signal("Physicsglass44");
					Game.Signal("Physicsglass46");
					Game.Signal("Physicsglass47");
					Game.Signal("Physicsglass48");
					Game.Signal("impulsesphere01");
				}
				if (Function == "explosion_002")
				{
					Game.Signal("Physicsglass34");
					Game.Signal("Physicsglass35");
					Game.Signal("Physicsglass36");
					Game.Signal("Physicsglass50");
					Game.Signal("Physicsglass51");
					Game.Signal("Physicsglass52");
					Game.Signal("impulsesphere02");
					Game.Signal("impulsesphere03");
					Game.Signal("impulsesphere04");
					Game.Signal("impulsesphere10");
					Game.Signal("impulsesphere11");
					Game.Signal("impulsesphere12");
				}
				if (Function == "explosion_003")
				{
					Game.Signal("Physicsglass30");
					Game.Signal("Physicsglass31");
					Game.Signal("Physicsglass32");
					Game.Signal("Physicsglass54");
					Game.Signal("Physicsglass55");
					Game.Signal("Physicsglass56");
					Game.Signal("impulsesphere13");
					Game.Signal("impulsesphere14");
					Game.Signal("impulsesphere15");
					Game.Signal("impulsesphere17");
					Game.Signal("impulsesphere18");
					Game.Signal("impulsesphere19");
				}
				if (Function == "explosion_004")
				{
					Game.Signal("Physicsglass26");
					Game.Signal("Physicsglass27");
					Game.Signal("Physicsglass28");
					Game.Signal("Physicsglass58");
					Game.Signal("Physicsglass59");
					Game.Signal("Physicsglass60");
					Game.Signal("impulsesphere21");
					Game.Signal("impulsesphere22");
					Game.Signal("impulsesphere23");
					Game.Signal("impulsesphere25");
					Game.Signal("impulsesphere26");
					Game.Signal("impulsesphere27");
				}
				if (Function == "explosion_005")
				{
					Game.Signal("Physicsglass22");
					Game.Signal("Physicsglass23");
					Game.Signal("Physicsglass24");
					Game.Signal("impulsesphere29");
					Game.Signal("impulsesphere30");
					Game.Signal("impulsesphere31");
				}
				if (Function == "BtoC")
				{
					Game.ChangeArea("csc_c_sn");
				}
				if (Function == "cscbridge01")
				{
					Game.Signal("objectphysics25");
				}
			}
			if (CharSection == "b_sd")
			{
				if (Function == "cage01")
				{
					Game.Signal("common_cage01");
				}
				if (Function == "glassbreak001")
				{
					Game.Signal("physicsglass01");
					Game.Signal("impulsesphere201");
				}
				if (Function == "glassbreak002")
				{
					Game.Signal("physicsglass02");
					Game.Signal("impulsesphere202");
				}
				if (Function == "glassbreak003")
				{
					Game.Signal("physicsglass03");
					Game.Signal("impulsesphere203");
				}
				if (Function == "glassbreak004")
				{
					Game.Signal("physicsglass04");
					Game.Signal("impulsesphere204");
				}
				if (Function == "glassbreak005")
				{
					Game.Signal("physicsglass05");
					Game.Signal("impulsesphere205");
				}
				if (Function == "glassbreak006")
				{
					Game.Signal("physicsglass06");
					Game.Signal("impulsesphere206");
				}
				if (Function == "glassbreak007")
				{
					Game.Signal("physicsglass07");
					Game.Signal("impulsesphere207");
				}
				if (Function == "glassbreak008")
				{
					Game.Signal("physicsglass08");
					Game.Signal("impulsesphere208");
				}
				if (Function == "glassbreak009")
				{
					Game.Signal("physicsglass09");
					Game.Signal("impulsesphere209");
				}
				if (Function == "glassbreak010")
				{
					Game.Signal("physicsglass10");
					Game.Signal("impulsesphere210");
				}
				if (Function == "glassbreak011")
				{
					Game.Signal("physicsglass11");
					Game.Signal("impulsesphere211");
				}
				if (Function == "glassbreak012")
				{
					Game.Signal("physicsglass12");
					Game.Signal("impulsesphere212");
				}
				if (Function == "explosion_001")
				{
					Game.Signal("bldgexplode01");
					Game.Signal("Physicsglass38");
					Game.Signal("Physicsglass39");
					Game.Signal("Physicsglass42");
					Game.Signal("Physicsglass43");
					Game.Signal("Physicsglass46");
					Game.Signal("Physicsglass47");
					Game.Signal("impulsesphere01");
				}
				if (Function == "explosion_002")
				{
					Game.Signal("Physicsglass34");
					Game.Signal("Physicsglass35");
					Game.Signal("Physicsglass50");
					Game.Signal("Physicsglass51");
					Game.Signal("impulsesphere03");
					Game.Signal("impulsesphere04");
					Game.Signal("impulsesphere10");
					Game.Signal("impulsesphere11");
				}
				if (Function == "explosion_003")
				{
					Game.Signal("Physicsglass30");
					Game.Signal("Physicsglass31");
					Game.Signal("Physicsglass54");
					Game.Signal("Physicsglass55");
					Game.Signal("impulsesphere14");
					Game.Signal("impulsesphere15");
					Game.Signal("impulsesphere18");
					Game.Signal("impulsesphere19");
				}
				if (Function == "explosion_004")
				{
					Game.Signal("Physicsglass26");
					Game.Signal("Physicsglass27");
					Game.Signal("Physicsglass58");
					Game.Signal("Physicsglass59");
					Game.Signal("impulsesphere22");
					Game.Signal("impulsesphere23");
					Game.Signal("impulsesphere26");
					Game.Signal("impulsesphere27");
				}
				if (Function == "explosion_005")
				{
					Game.Signal("Physicsglass62");
					Game.Signal("Physicsglass63");
					Game.Signal("impulsesphere33");
					Game.Signal("impulsesphere34");
				}
				if (Function == "BtoC")
				{
					Game.ChangeArea("csc_c_sd");
				}
				if (Function == "cscbridge01")
				{
					Game.Signal("objectphysics25");
				}
			}
			if (CharSection == "b_sv")
			{
				if (Function == "cage01")
				{
					Game.Signal("common_cage01");
				}
				if (Function == "cage02")
				{
					Game.Signal("common_cage02");
				}
				if (Function == "explosion_001")
				{
					Game.Signal("bldgexplode01");
					Game.Signal("impulsesphere01");
					Game.Signal("Physicsglass38");
					Game.Signal("Physicsglass42");
					Game.Signal("Physicsglass46");
					Game.Signal("bldgexplode02");
					Game.Signal("impulsesphere41");
					Game.Signal("Physicsglass85");
					Game.Signal("Physicsglass89");
					Game.Signal("Physicsglass93");
				}
				if (Function == "explosion_002")
				{
					Game.Signal("impulsesphere02");
					Game.Signal("impulsesphere03");
					Game.Signal("impulsesphere04");
					Game.Signal("impulsesphere05");
					Game.Signal("impulsesphere06");
					Game.Signal("impulsesphere07");
					Game.Signal("impulsesphere08");
					Game.Signal("impulsesphere09");
					Game.Signal("impulsesphere10");
					Game.Signal("impulsesphere11");
					Game.Signal("impulsesphere12");
					Game.Signal("Physicsglass33");
					Game.Signal("Physicsglass36");
					Game.Signal("Physicsglass45");
					Game.Signal("Physicsglass51");
					Game.Signal("Physicsglass81");
					Game.Signal("Physicsglass84");
					Game.Signal("Physicsglass96");
					Game.Signal("Physicsglass99");
				}
				if (Function == "explosion_003")
				{
					Game.Signal("impulsesphere13");
					Game.Signal("impulsesphere14");
					Game.Signal("impulsesphere15");
					Game.Signal("impulsesphere16");
					Game.Signal("impulsesphere17");
					Game.Signal("impulsesphere18");
					Game.Signal("impulsesphere19");
					Game.Signal("impulsesphere20");
					Game.Signal("Physicsglass29");
					Game.Signal("Physicsglass32");
					Game.Signal("Physicsglass55");
					Game.Signal("Physicsglass78");
					Game.Signal("Physicsglass102");
					Game.Signal("Physicsglass104");
				}
				if (Function == "explosion_004")
				{
					Game.Signal("impulsesphere21");
					Game.Signal("impulsesphere22");
					Game.Signal("impulsesphere23");
					Game.Signal("impulsesphere24");
					Game.Signal("impulsesphere25");
					Game.Signal("impulsesphere26");
					Game.Signal("impulsesphere27");
					Game.Signal("impulsesphere28");
					Game.Signal("Physicsglass25");
					Game.Signal("Physicsglass28");
					Game.Signal("Physicsglass59");
					Game.Signal("Physicsglass73");
					Game.Signal("Physicsglass76");
					Game.Signal("Physicsglass107");
				}
				if (Function == "explosion_005")
				{
					Game.Signal("impulsesphere29");
					Game.Signal("impulsesphere30");
					Game.Signal("impulsesphere31");
					Game.Signal("impulsesphere32");
					Game.Signal("impulsesphere33");
					Game.Signal("impulsesphere34");
					Game.Signal("impulsesphere35");
					Game.Signal("impulsesphere36");
					Game.Signal("Physicsglass21");
					Game.Signal("Physicsglass24");
					Game.Signal("Physicsglass63");
					Game.Signal("Physicsglass70");
					Game.Signal("Physicsglass109");
					Game.Signal("Physicsglass112");
				}
				if (Function == "explosion_006")
				{
					Game.Signal("impulsesphere37");
					Game.Signal("impulsesphere38");
					Game.Signal("impulsesphere39");
					Game.Signal("impulsesphere40");
					Game.Signal("Physicsglass65");
					Game.Signal("Physicsglass67");
					Game.Signal("Physicsglass113");
					Game.Signal("Physicsglass115");
				}
				if (Function == "BtoF2")
				{
					Game.ChangeArea("csc_f2_sv");
				}
				if (Function == "bridge1000")
				{
					Game.Signal("objectphysics10");
				}
			}
			if (CharSection == "c_sn" && Function == "CtoE")
			{
				Game.ChangeArea("csc_e_sn");
			}
			if (CharSection == "c_sd")
			{
				if (Function == "CtoF")
				{
					Game.ChangeArea("csc_f_sd");
				}
				if (Function == "signal/physicspath53")
				{
					Game.Signal("physicspath53");
				}
				if (Function == "signal/physicspath54")
				{
					Game.Signal("physicspath54");
				}
			}
			if (CharSection == "e_sn")
			{
				if (Function == "car008")
				{
					Game.ProcessMessage("tornado01", "TornadoShoot", new TornadoChaseParams(1, new Vector3(2200f, 500f, -200f) * 0.0105f));
				}
				if (Function == "car009")
				{
					Game.ProcessMessage("tornado01", "TornadoShoot", new TornadoChaseParams(3, new Vector3(2000f, 500f, 0f) * 0.0105f));
				}
				if (Function == "car010")
				{
					Game.ProcessMessage("tornado01", "TornadoShoot", new TornadoChaseParams(3, new Vector3(2000f, 500f, 200f) * 0.0105f));
				}
				if (Function == "car015")
				{
					Game.ProcessMessage("tornado01", "TornadoShoot", new TornadoChaseParams(1, new Vector3(-1000f, -200f, 200f) * 0.0105f));
				}
				if (Function == "car016")
				{
					Game.ProcessMessage("tornado01", "TornadoShoot", new TornadoChaseParams(3, new Vector3(-600f, -100f, -200f) * 0.0105f));
				}
				if (Function == "car017")
				{
					Game.ProcessMessage("tornado01", "TornadoShoot", new TornadoChaseParams(1, new Vector3(-1600f, -200f, 600f) * 0.0105f));
				}
				if (Function == "car018")
				{
					Game.ProcessMessage("tornado01", "TornadoShoot", new TornadoChaseParams(3, new Vector3(0f, 0f, 0f) * 0.0105f));
				}
				if (Function == "car021")
				{
					Game.ProcessMessage("tornado01", "TornadoShoot", new TornadoChaseParams(1, new Vector3(0f, 0f, 0f) * 0.0105f));
				}
				if (Function == "car023")
				{
					Game.ProcessMessage("tornado01", "TornadoShoot", new TornadoChaseParams(2, new Vector3(0f, 0f, 0f) * 0.0105f));
				}
				if (Function == "car032")
				{
					Game.ProcessMessage("tornado01", "TornadoShoot", new TornadoChaseParams(3, new Vector3(0f, 0f, 0f) * 0.0105f));
				}
				if (Function == "car034")
				{
					Game.ProcessMessage("tornado01", "TornadoShoot", new TornadoChaseParams(2, new Vector3(0f, 0f, 0f) * 0.0105f));
				}
				if (Function == "car035")
				{
					Game.ProcessMessage("tornado01", "TornadoShoot", new TornadoChaseParams(1, new Vector3(0f, 0f, 0f) * 0.0105f));
				}
				if (Function == "car036")
				{
					Game.ProcessMessage("tornado01", "TornadoShoot", new TornadoChaseParams(1, new Vector3(0f, 0f, 0f) * 0.0105f));
				}
				if (Function == "trailer001")
				{
					Game.ProcessMessage("tornado01", "TornadoShoot", new TornadoChaseParams(0, new Vector3(160014f, 10974.157f, 0f) * 0.0105f));
				}
				if (Function == "billboard001")
				{
					Game.NewActor("DefaultPrefabs/Objects/CSC/CscFlybillboard", new FlyBillboardParams(125f, "line_path001"));
				}
				if (Function == "billboard002")
				{
					Game.NewActor("DefaultPrefabs/Objects/CSC/CscFlybillboard", new FlyBillboardParams(125f, "line_path002"));
				}
			}
			if (CharSection == "f_sd")
			{
				if (Function == "t001")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(1, null, "t001", 2.5f, Vector3.zero));
				}
				if (Function == "t002")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(2, null, "t002", 2.5f, Vector3.zero));
				}
				if (Function == "t003")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(1, null, "t003", 2.2f, Vector3.zero));
				}
				if (Function == "t004")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(3, null, "t004", 2f, Vector3.zero));
				}
				if (Function == "t005")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(2, null, "t005", 2f, Vector3.zero));
				}
				if (Function == "t006")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(2, null, "t006", 1.5f, Vector3.zero));
				}
				if (Function == "t007")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(1, null, "t007", 1.5f, Vector3.zero));
				}
				if (Function == "t100")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(1, null, "t100", 0f, Vector3.zero));
				}
				if (Function == "t101")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(2, null, "t101", 0f, Vector3.zero));
				}
				if (Function == "t102")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(1, null, "t102", 0f, Vector3.zero));
				}
				if (Function == "t103")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(3, null, "t103", 0f, Vector3.zero));
				}
				if (Function == "t104")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(2, null, "t104", 0f, Vector3.zero));
				}
				if (Function == "t105")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(2, null, "t105", 0f, Vector3.zero));
				}
				if (Function == "t106")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(1, null, "t106", 0f, Vector3.zero));
				}
				if (Function == "t107")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(1, null, "t107", 0f, Vector3.zero));
				}
				if (Function == "signal/objectphysics46")
				{
					Game.Signal("objectphysics46");
				}
				if (Function == "signal/objectphysics85")
				{
					Game.Signal("objectphysics85");
				}
				if (Function == "cscbridge01")
				{
					Game.Signal("objectphysics107");
				}
			}
			if (CharSection == "f1_sv")
			{
				if (Function == "t001")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(1, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "t002")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(1, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "t003")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(2, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "t005")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(2, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "t006")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(1, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "esp01")
				{
					Game.Signal("physicspath01");
					Game.ProcessMessage("physicspath01", "PsiEffect", true);
					Game.Signal("physicspath02");
					Game.ProcessMessage("physicspath02", "PsiEffect", true);
					Game.Signal("physicspath03");
					Game.ProcessMessage("physicspath03", "PsiEffect", true);
				}
				if (Function == "esp02")
				{
					Game.Signal("physicspath05");
					Game.ProcessMessage("physicspath05", "PsiEffect", true);
					Game.Signal("physicspath04");
					Game.ProcessMessage("physicspath04", "PsiEffect", true);
					Game.Signal("physicspath06");
					Game.ProcessMessage("physicspath06", "PsiEffect", true);
				}
				if (Function == "esp03")
				{
					Game.Signal("physicspath07");
					Game.ProcessMessage("physicspath07", "PsiEffect", true);
					Game.Signal("physicspath08");
					Game.ProcessMessage("physicspath08", "PsiEffect", true);
					Game.Signal("physicspath09");
					Game.ProcessMessage("physicspath09", "PsiEffect", true);
					Game.Signal("physicspath10");
					Game.ProcessMessage("physicspath10", "PsiEffect", true);
					Game.Signal("physicspath11");
					Game.ProcessMessage("physicspath11", "PsiEffect", true);
				}
				if (Function == "esp04")
				{
					Game.Signal("physicspath12");
					Game.ProcessMessage("physicspath12", "PsiEffect", true);
					Game.Signal("physicspath13");
					Game.ProcessMessage("physicspath13", "PsiEffect", true);
					Game.Signal("physicspath14");
					Game.ProcessMessage("physicspath14", "PsiEffect", true);
					Game.Signal("physicspath15");
					Game.ProcessMessage("physicspath15", "PsiEffect", true);
				}
				if (Function == "glassbreak001")
				{
					Game.Signal("objectphysics338");
					Game.Signal("impulsesphere01");
				}
				if (Function == "glassbreak002")
				{
					Game.Signal("objectphysics339");
					Game.Signal("impulsesphere02");
				}
				if (Function == "glassbreak003")
				{
					Game.Signal("objectphysics340");
					Game.Signal("impulsesphere03");
				}
				if (Function == "glassbreak004")
				{
					Game.Signal("objectphysics341");
					Game.Signal("impulsesphere04");
				}
				if (Function == "glassbreak005")
				{
					Game.Signal("objectphysics342");
					Game.Signal("objectphysics343");
					Game.Signal("impulsesphere05");
				}
				if (Function == "F1toB")
				{
					Game.ChangeArea("csc_b_sv");
				}
				if (Function == "cage01")
				{
					Game.Signal("common_cage01");
				}
				if (Function == "glass1000")
				{
					Game.Signal("objectphysics1000");
					Game.Signal("impulsesphere1000");
				}
			}
			if (CharSection == "f2_sv")
			{
				if (Function == "t001")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(1, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "t002")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(2, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "t007")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(1, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "t008")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(1, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "t009")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(1, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "t010")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(2, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "t011")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(3, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "t012")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(2, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "t013")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(3, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "t014")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(1, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "t015")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(1, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "t016")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(3, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "signal/objectphysics116")
				{
					Game.Signal("objectphysics116");
				}
				if (Function == "signal/objectphysics121")
				{
					Game.Signal("objectphysics121");
				}
			}
			if (CharSection == "f_sv")
			{
				if (Function == "t001")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(1, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "t003")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(2, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "t005")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(2, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "t006")
				{
					Game.ProcessMessage("csctornado01", "TornadoShoot", new TornadoParams(1, Transmitter, "", 0f, Vector3.zero));
				}
				if (Function == "esp01")
				{
					Game.Signal("physicspath01");
					Game.ProcessMessage("physicspath01", "PsiEffect", true);
					Game.Signal("physicspath02");
					Game.ProcessMessage("physicspath02", "PsiEffect", true);
					Game.Signal("physicspath03");
					Game.ProcessMessage("physicspath03", "PsiEffect", true);
				}
				if (Function == "esp02")
				{
					Game.Signal("physicspath05");
					Game.ProcessMessage("physicspath05", "PsiEffect", true);
					Game.Signal("physicspath04");
					Game.ProcessMessage("physicspath04", "PsiEffect", true);
					Game.Signal("physicspath06");
					Game.ProcessMessage("physicspath06", "PsiEffect", true);
				}
				if (Function == "glassbreak001")
				{
					Game.Signal("objectphysics338");
					Game.Signal("impulsesphere01");
				}
				if (Function == "glassbreak002")
				{
					Game.Signal("objectphysics339");
					Game.Signal("impulsesphere02");
				}
				if (Function == "glassbreak003")
				{
					Game.Signal("objectphysics340");
					Game.Signal("impulsesphere03");
				}
				if (Function == "glassbreak004")
				{
					Game.Signal("objectphysics341");
					Game.Signal("impulsesphere04");
				}
				if (Function == "glassbreak005")
				{
					Game.Signal("objectphysics342");
					Game.Signal("objectphysics343");
					Game.Signal("impulsesphere05");
				}
			}
		}
		if (Stage == "flc")
		{
			if (CharSection == "a_sn")
			{
				if (Function == "AtoB")
				{
					Game.ChangeArea("flc_b_sn");
				}
				if (Function == "door_open03")
				{
					Game.ProcessMessage("flc_door01", "GateOpen");
				}
				if (Function == "door_open04")
				{
					Game.ProcessMessage("flc_door02", "GateOpen");
				}
				if (Function == "door_open01")
				{
					Game.ProcessMessage("flc_door03", "GateOpen");
				}
				if (Function == "door_open02")
				{
					Game.ProcessMessage("flc_door04", "GateOpen");
				}
				if (Function == "inclined01")
				{
					Game.Signal("inclinedstonebridge02");
				}
				if (Function == "inclined02")
				{
					Game.Signal("inclinedstonebridge01");
				}
				if (Function == "cage01")
				{
					Game.Signal("common_cage01");
				}
				if (Function == "cage05")
				{
					Game.Signal("common_cage05");
				}
				if (Function == "cage06")
				{
					Game.Signal("common_cage06");
				}
			}
			if (CharSection == "a_sd")
			{
				if (Function == "AtoB")
				{
					Game.ChangeArea("flc_b_sd");
				}
				if (Function == "door_open03")
				{
					Game.ProcessMessage("flc_door01", "GateOpen");
				}
				if (Function == "door_open04")
				{
					Game.ProcessMessage("flc_door02", "GateOpen");
				}
				if (Function == "door_open01")
				{
					Game.ProcessMessage("flc_door03", "GateOpen");
				}
				if (Function == "door_open02")
				{
					Game.ProcessMessage("flc_door04", "GateOpen");
				}
				if (Function == "inclined01")
				{
					Game.Signal("inclinedstonebridge02");
				}
				if (Function == "inclined02")
				{
					Game.Signal("inclinedstonebridge01");
				}
				if (Function == "cage02")
				{
					Game.Signal("common_cage03");
				}
				if (Function == "cage03")
				{
					Game.Signal("common_cage08");
				}
				if (Function == "cage06")
				{
					Game.Signal("common_cage02");
				}
				if (Function == "cage08")
				{
					Game.Signal("common_cage11");
				}
				if (Function == "cage10")
				{
					Game.Signal("common_cage05");
				}
				if (Function == "door_open/flc_door04")
				{
					Game.ProcessMessage("flc_door04", "GateClose");
				}
				if (Function == "door_open/flc_door02")
				{
					Game.ProcessMessage("flc_door02", "GateClose");
				}
			}
			if (CharSection == "a_sv")
			{
				if (Function == "AtoB")
				{
					Game.ChangeArea("flc_b_sv");
				}
				if (Function == "move_floor01")
				{
					Game.Signal("physicspath234");
					Game.ProcessMessage("physicspath234", "PsiEffect", true);
					Game.Signal("physicspath235");
					Game.ProcessMessage("physicspath235", "PsiEffect", true);
				}
				if (Function == "move_floor02")
				{
					Game.Signal("physicspath36");
					Game.ProcessMessage("physicspath36", "PsiEffect", true);
					Game.Signal("physicspath231");
					Game.ProcessMessage("physicspath231", "PsiEffect", true);
				}
				if (Function == "move_floor03")
				{
					Game.Signal("physicspath232");
					Game.ProcessMessage("physicspath232", "PsiEffect", true);
					Game.Signal("physicspath237");
					Game.ProcessMessage("physicspath237", "PsiEffect", true);
				}
				if (Function == "move_floor04")
				{
					Game.Signal("physicspath238");
					Game.ProcessMessage("physicspath238", "PsiEffect", true);
					Game.Signal("physicspath239");
					Game.ProcessMessage("physicspath239", "PsiEffect", true);
				}
				if (Function == "move_floor05")
				{
					Game.Signal("physicspath240");
					Game.ProcessMessage("physicspath240", "PsiEffect", true);
					Game.Signal("physicspath241");
					Game.ProcessMessage("physicspath241", "PsiEffect", true);
				}
				if (Function == "move_floor06")
				{
					Game.Signal("physicspath50");
					Game.ProcessMessage("physicspath50", "PsiEffect", true);
					Game.Signal("physicspath242");
					Game.ProcessMessage("physicspath242", "PsiEffect", true);
				}
				if (Function == "move_floor07")
				{
					Game.Signal("physicspath247");
					Game.ProcessMessage("physicspath247", "PsiEffect", true);
				}
				if (Function == "door_open01")
				{
					Game.ProcessMessage("flc_door03", "GateOpen");
				}
				if (Function == "door_open02")
				{
					Game.ProcessMessage("flc_door04", "GateOpen");
					Game.ProcessMessage("flc_door04", "PsiEffect", true);
				}
				if (Function == "door_open03")
				{
					Game.ProcessMessage("flc_door01", "GateOpen");
				}
				if (Function == "door_open04")
				{
					Game.ProcessMessage("flc_door02", "GateOpen");
					Game.ProcessMessage("flc_door02", "PsiEffect", true);
				}
				if (Function == "move_floor08")
				{
					Game.Signal("physicspath37");
					Game.ProcessMessage("physicspath37", "PsiEffect", true);
				}
				if (Function == "move_floor09")
				{
					Game.Signal("physicspath255");
					Game.ProcessMessage("physicspath255", "PsiEffect", true);
				}
				if (Function == "move_floor10")
				{
					Game.Signal("physicspath256");
					Game.ProcessMessage("physicspath256", "PsiEffect", true);
					Game.Signal("physicspath257");
					Game.ProcessMessage("physicspath257", "PsiEffect", true);
				}
				if (Function == "move_floor11")
				{
					Game.Signal("physicspath251");
					Game.ProcessMessage("physicspath251", "PsiEffect", true);
					Game.Signal("physicspath252");
					Game.ProcessMessage("physicspath252", "PsiEffect", true);
					Game.Signal("physicspath253");
					Game.ProcessMessage("physicspath253", "PsiEffect", true);
				}
				if (Function == "move_floor12")
				{
					Game.Signal("physicspath259");
					Game.ProcessMessage("physicspath259", "PsiEffect", true);
				}
				if (Function == "cage01")
				{
					Game.Signal("common_cage01");
				}
				if (Function == "cage02")
				{
					Game.Signal("common_cage03");
				}
				if (Function == "cage03")
				{
					Game.Signal("common_cage02");
				}
				if (Function == "cage04")
				{
					Game.Signal("common_cage04");
				}
				if (Function == "cage05")
				{
					Game.Signal("common_cage05");
				}
			}
			if (CharSection == "b_sn")
			{
				if (Function == "door_open00")
				{
					Game.ProcessMessage("flc_door01", "GateOpen");
				}
				if (Function == "door_close00")
				{
					Game.ProcessMessage("flc_door01", "GateClose");
				}
				if (Function == "door_open01")
				{
					Game.ProcessMessage("flc_door03", "GateOpen");
				}
				if (Function == "door_close01")
				{
					Game.ProcessMessage("flc_door03", "GateClose");
				}
				if (Function == "door_open02")
				{
					Game.Signal("flamesequence01");
					Game.Signal("flamesequence02");
					Game.Signal("flamesequence03");
					Game.Signal("flamesequence04");
					Game.Signal("flamesequence05");
					Game.ProcessMessage("flc_door04", "GateOpen");
					Game.Signal("DestroyAmigoObjects");
				}
				if (Function == "door_close02")
				{
					Game.ProcessMessage("flc_door04", "GateClose");
				}
				if (Function == "cage03")
				{
					Game.Signal("common_cage03");
				}
				if (Function == "itemboxg")
				{
					Game.StartEntityByName("GroupHelper100");
				}
			}
			if (CharSection == "b_sd")
			{
				if (Function == "door_open00")
				{
					Game.ProcessMessage("flc_door01", "GateOpen");
				}
				if (Function == "door_close00")
				{
					Game.ProcessMessage("flc_door01", "GateClose");
				}
				if (Function == "door_open01")
				{
					Game.ProcessMessage("flc_door03", "GateOpen");
				}
				if (Function == "door_close01")
				{
					Game.ProcessMessage("flc_door03", "GateClose");
				}
				if (Function == "door_open02")
				{
					Game.Signal("flamesequence01");
					Game.Signal("flamesequence02");
					Game.Signal("flamesequence03");
					Game.Signal("flamesequence04");
					Game.Signal("flamesequence05");
					Game.ProcessMessage("flc_door04", "GateOpen");
					Game.Signal("DestroyAmigoObjects");
				}
				if (Function == "door_close02")
				{
					Game.ProcessMessage("flc_door04", "GateClose");
				}
				if (Function == "cage02")
				{
					Game.Signal("common_cage02");
				}
				if (Function == "cage03")
				{
					Game.Signal("common_cage03");
				}
				if (Function == "cage04")
				{
					Game.Signal("common_cage04");
				}
				if (Function == "cage05")
				{
					Game.Signal("common_cage05");
				}
				if (Function == "itemboxg")
				{
					Game.StartEntityByName("GroupHelper100");
				}
			}
			if (CharSection == "b_sv")
			{
				if (Function == "move_floor01")
				{
					Game.Signal("physicspath14");
					Game.ProcessMessage("physicspath14", "PsiEffect", true);
					Game.Signal("physicspath15");
					Game.ProcessMessage("physicspath15", "PsiEffect", true);
					Game.Signal("physicspath16");
					Game.ProcessMessage("physicspath16", "PsiEffect", true);
				}
				if (Function == "move_floor02")
				{
					Game.Signal("physicspath124");
					Game.ProcessMessage("physicspath124", "PsiEffect", true);
					Game.Signal("physicspath141");
					Game.ProcessMessage("physicspath141", "PsiEffect", true);
				}
				if (Function == "move_floor03")
				{
					Game.Signal("physicspath129");
					Game.ProcessMessage("physicspath129", "PsiEffect", true);
					Game.Signal("physicspath130");
					Game.ProcessMessage("physicspath130", "PsiEffect", true);
				}
				if (Function == "move_floor04")
				{
					Game.Signal("physicspath131");
					Game.ProcessMessage("physicspath131", "PsiEffect", true);
				}
				if (Function == "move_floor05")
				{
					Game.Signal("physicspath132");
					Game.ProcessMessage("physicspath132", "PsiEffect", true);
				}
				if (Function == "move_floor06")
				{
					Game.Signal("physicspath20");
					Game.ProcessMessage("physicspath20", "PsiEffect", true);
				}
				if (Function == "move_floor07")
				{
					Game.Signal("physicspath21");
					Game.ProcessMessage("physicspath21", "PsiEffect", true);
				}
				if (Function == "move_floor08")
				{
					Game.Signal("physicspath22");
					Game.ProcessMessage("physicspath22", "PsiEffect", true);
				}
				if (Function == "move_floor09")
				{
					Game.Signal("physicspath24");
					Game.ProcessMessage("physicspath24", "PsiEffect", true);
				}
				if (Function == "move_floor10")
				{
					Game.Signal("physicspath25");
					Game.ProcessMessage("physicspath25", "PsiEffect", true);
				}
				if (Function == "move_floor11")
				{
					Game.Signal("physicspath134");
					Game.ProcessMessage("physicspath134", "PsiEffect", true);
				}
				if (Function == "move_floor12")
				{
					Game.Signal("physicspath135");
					Game.ProcessMessage("physicspath135", "PsiEffect", true);
					Game.Signal("physicspath26");
					Game.ProcessMessage("physicspath26", "PsiEffect", true);
				}
				if (Function == "move_floor13")
				{
					Game.Signal("physicspath04");
					Game.ProcessMessage("physicspath04", "PsiEffect", true);
				}
				if (Function == "move_floor14")
				{
					Game.Signal("physicspath08");
					Game.ProcessMessage("physicspath08", "PsiEffect", true);
				}
				if (Function == "move_floor15")
				{
					Game.Signal("physicspath03");
					Game.ProcessMessage("physicspath03", "PsiEffect", true);
				}
				if (Function == "move_floor16")
				{
					Game.Signal("physicspath30");
					Game.ProcessMessage("physicspath30", "PsiEffect", true);
				}
				if (Function == "move_floor17")
				{
					Game.Signal("physicspath06");
					Game.ProcessMessage("physicspath06", "PsiEffect", true);
					Game.Signal("physicspath31");
					Game.ProcessMessage("physicspath31", "PsiEffect", true);
				}
				if (Function == "move_floor18")
				{
					Game.Signal("physicspath140");
					Game.ProcessMessage("physicspath140", "PsiEffect", true);
				}
				if (Function == "door_open02")
				{
					Game.ProcessMessage("flc_door01", "GateOpen");
					Game.ProcessMessage("flc_door01", "PsiEffect", true);
				}
				if (Function == "door_close02")
				{
					Game.ProcessMessage("flc_door01", "GateClose");
				}
				if (Function == "door_open01")
				{
					Game.ProcessMessage("flc_door02", "GateOpen");
					Game.ProcessMessage("flc_door02", "PsiEffect", true);
					Game.StartEntityByName("common_hint_collision05");
				}
				if (Function == "door_close01")
				{
					Game.ProcessMessage("flc_door02", "GateClose");
				}
				if (Function == "door_open03")
				{
					Game.ProcessMessage("flc_door04", "GateOpen");
					Game.ProcessMessage("flc_door04", "PsiEffect", true);
				}
				if (Function == "door_close03")
				{
					Game.ProcessMessage("flc_door04", "GateClose");
				}
				if (Function == "door_open04")
				{
					Game.ProcessMessage("flc_door05", "GateOpen");
					Game.ProcessMessage("flc_door05", "PsiEffect", true);
				}
				if (Function == "door_close04")
				{
					Game.ProcessMessage("flc_door05", "GateClose");
				}
				if (Function == "door_open05")
				{
					Game.ProcessMessage("flc_door06", "GateOpen");
					Game.ProcessMessage("flc_door06", "PsiEffect", true);
				}
				if (Function == "door_close05")
				{
					Game.ProcessMessage("flc_door06", "GateClose");
				}
				if (Function == "door_close06")
				{
					Game.ProcessMessage("flc_door07", "GateClose");
				}
				if (Function == "flamecore_on_1")
				{
					Game.ProcessMessage("flc_flamecore01", "TornadoShoot", new FlameCoreParams(1, Transmitter));
				}
				if (Function == "flamecore_on_2")
				{
					Game.ProcessMessage("flc_flamecore01", "TornadoShoot", new FlameCoreParams(2, Transmitter));
				}
				if (Function == "flamecore_off")
				{
					Game.ProcessMessage("flc_flamecore01", "TornadoShoot", new FlameCoreParams(0, Transmitter));
				}
				if (Function == "flamecool")
				{
					Game.Signal("freezedmantle01");
					Game.ProcessMessage("flc_door07", "GateOpen");
					Game.StartEntityByName("cameraeventbox127");
					Game.Signal("DestroyArenaObjects");
				}
				if (Function == "cage01")
				{
					Game.Signal("common_cage01");
				}
				if (Function == "itemboxg")
				{
					Game.StartEntityByName("GroupHelper100");
				}
				if (Function == "move_floor1000")
				{
					Game.Signal("physicspath1000");
					Game.Signal("physicspath1001");
				}
				if (Function == "upgradeflamecool")
				{
					Game.Signal("freezedmantle01");
					Game.ProcessMessage("flc_door07", "GateOpen");
					Game.StartEntityByName("cameraeventbox127_upgrade");
					Game.Signal("DestroyArenaObjects");
				}
			}
		}
		if (Stage == "rct")
		{
			if (CharSection == "a_sn")
			{
				if (Function == "AtoB")
				{
					Game.ChangeArea("rct_b_sn");
				}
				if (Function == "train_start01")
				{
					Game.ProcessMessage("normal_train02", "Go");
					Game.ProcessMessage("normal_train02", "Horn");
				}
				if (Function == "train_horn01")
				{
					Game.ProcessMessage("normal_train02", "Horn");
				}
				if (Function == "train_horn02")
				{
					Game.ProcessMessage("normal_train02", "Horn");
				}
				if (Function == "train_camera01")
				{
					Game.ProcessMessage("normal_train02", "Horn");
				}
				if (Function == "train_brake01")
				{
					Game.Signal("objectphysics80");
					Game.ProcessMessage("normal_train02", "Camera");
					Game.ProcessMessage("normal_train02", "Bomb");
					Game.ProcessMessage("normal_train02", "Stop");
				}
				if (Function == "door_open01")
				{
					Game.Signal("rct_door01");
					Game.Signal("objectphysics80");
					Game.ProcessMessage("common_object_event01", "GateClose");
					Game.ProcessMessage("common_object_event04", "GateClose");
					Game.ProcessMessage("common_object_event07", "GateClose");
					Game.ProcessMessage("common_object_event08", "GateClose");
				}
				if (Function == "train_start02")
				{
					Game.ProcessMessage("normal_train01", "Go");
					Game.ProcessMessage("normal_train01", "Horn");
				}
				if (Function == "train_horn03")
				{
					Game.ProcessMessage("normal_train01", "Horn");
				}
				if (Function == "train_horn04")
				{
					Game.ProcessMessage("normal_train01", "Horn");
				}
				if (Function == "train_camera02")
				{
					Game.ProcessMessage("normal_train01", "Horn");
				}
				if (Function == "train_brake02")
				{
					Game.Signal("objectphysics316");
					Game.ProcessMessage("normal_train01", "Camera");
					Game.ProcessMessage("normal_train01", "Bomb");
					Game.ProcessMessage("normal_train01", "Stop");
				}
				if (Function == "door_open02")
				{
					Game.Signal("rct_door02");
					Game.Signal("objectphysics316");
					Game.ProcessMessage("common_object_event02", "GateClose");
					Game.ProcessMessage("common_object_event05", "GateClose");
					Game.ProcessMessage("common_object_event09", "GateClose");
					Game.ProcessMessage("common_object_event10", "GateClose");
				}
				if (Function == "train_start03")
				{
					Game.ProcessMessage("normal_train03", "Go");
					Game.ProcessMessage("normal_train03", "Horn");
				}
				if (Function == "train_horn05")
				{
					Game.ProcessMessage("normal_train03", "Horn");
				}
				if (Function == "train_horn06")
				{
					Game.ProcessMessage("normal_train03", "Horn");
				}
				if (Function == "train_camera03")
				{
					Game.ProcessMessage("normal_train03", "Horn");
				}
				if (Function == "train_brake03")
				{
					Game.Signal("objectphysics338");
					Game.ProcessMessage("normal_train03", "Camera");
					Game.ProcessMessage("normal_train03", "Bomb");
					Game.ProcessMessage("normal_train03", "Stop");
				}
				if (Function == "door_open03")
				{
					Game.Signal("rct_door03");
					Game.Signal("objectphysics338");
					Game.ProcessMessage("common_object_event03", "GateClose");
					Game.ProcessMessage("common_object_event06", "GateClose");
					Game.ProcessMessage("common_object_event11", "GateClose");
					Game.ProcessMessage("common_object_event12", "GateClose");
				}
				if (Function == "train_start04")
				{
					Game.ProcessMessage("freight_train03", "Go");
					Game.ProcessMessage("freight_train03", "Horn");
				}
				if (Function == "train_start05")
				{
					Game.ProcessMessage("freight_train04", "Go");
					Game.ProcessMessage("freight_train05", "Go");
					Game.ProcessMessage("freight_train04", "Horn");
					Game.ProcessMessage("freight_train05", "Horn");
				}
				if (Function == "train_start06")
				{
					Game.ProcessMessage("freight_train01", "Go");
					Game.ProcessMessage("freight_train02", "Go");
					Game.ProcessMessage("freight_train01", "Horn");
					Game.ProcessMessage("freight_train02", "Horn");
				}
				if (Function == "cage01")
				{
					Game.Signal("common_cage01");
				}
			}
			if (CharSection == "a_sd")
			{
				if (Function == "AtoB")
				{
					Game.ChangeArea("rct_b_sd");
				}
				if (Function == "cage01")
				{
					Game.Signal("common_cage01");
				}
				if (Function == "cage02")
				{
					Game.Signal("common_cage02");
				}
				if (Function == "train_start01")
				{
					Game.ProcessMessage("eggman_train01", "Go");
					Game.ProcessMessage("common_laser01", "GateOpen");
					Game.ProcessMessage("common_laser02", "GateOpen");
					Game.ProcessMessage("common_object_event01", "GateClose");
					Game.ProcessMessage("common_object_event03", "GateOpen");
					Game.ProcessMessage("common_object_event04", "GateOpen");
					Game.ProcessMessage("eggman_train01", "Horn");
				}
				if (Function == "door_open01")
				{
					Game.ProcessMessage("common_laser01", "GateClose");
					Game.ProcessMessage("common_laser02", "GateClose");
					Game.ProcessMessage("common_laser03", "GateOpen");
					Game.ProcessMessage("common_laser04", "GateOpen");
					Game.ProcessMessage("eggman_train01", "Fast");
					Game.ProcessMessage("common_object_event01", "GateOpen");
					Game.ProcessMessage("common_object_event03", "GateClose");
					Game.ProcessMessage("common_object_event04", "GateClose");
					Game.ProcessMessage("eggman_train01", "Horn");
					Game.ProcessMessage("common_object_event07", "GateClose");
					Game.ProcessMessage("common_object_event08", "GateClose");
					Game.StartEntityByName("Barrier01Close");
				}
				if (Function == "train_brake01")
				{
					Game.ProcessMessage("eggman_train01", "Stop");
					Game.ProcessMessage("eggman_train01", "Horn");
				}
				if (Function == "door_open03")
				{
					Game.ProcessMessage("eggman_train01", "Go");
					Game.ProcessMessage("eggman_train01", "Fast");
					Game.ProcessMessage("rct_door01", "GateOpen");
					Game.ProcessMessage("common_laser01", "GateOpen");
					Game.ProcessMessage("common_laser02", "GateOpen");
					Game.ProcessMessage("common_laser03", "GateClose");
					Game.ProcessMessage("common_laser04", "GateClose");
					Game.ProcessMessage("common_object_event01", "GateClose");
					Game.ProcessMessage("common_object_event03", "GateClose");
					Game.ProcessMessage("common_object_event04", "GateClose");
					Game.StartEntityByName("Barrier01Open");
				}
				if (Function == "train_horn01")
				{
					Game.ProcessMessage("eggman_train01", "Horn");
				}
				if (Function == "train_horn02")
				{
					Game.ProcessMessage("eggman_train01", "Horn");
				}
				if (Function == "train_camera01")
				{
					Game.ProcessMessage("eggman_train01", "Camera");
					Game.ProcessMessage("eggman_train01", "Fast");
					Game.ProcessMessage("eggman_train01", "Horn");
					Game.StartEntityByName("TrainEscape");
				}
				if (Function == "train_bust01")
				{
					Game.ProcessMessage("eggman_train01", "Bomb");
				}
				if (Function == "train_start02")
				{
					Game.ProcessMessage("eggman_train02", "Go");
					Game.ProcessMessage("common_laser07", "GateOpen");
					Game.ProcessMessage("common_laser08", "GateOpen");
					Game.ProcessMessage("common_object_event02", "GateClose");
					Game.ProcessMessage("common_object_event05", "GateOpen");
					Game.ProcessMessage("common_object_event06", "GateOpen");
					Game.ProcessMessage("eggman_train02", "Horn");
				}
				if (Function == "door_open02")
				{
					Game.ProcessMessage("common_laser07", "GateClose");
					Game.ProcessMessage("common_laser08", "GateClose");
					Game.ProcessMessage("common_laser05", "GateOpen");
					Game.ProcessMessage("common_laser06", "GateOpen");
					Game.ProcessMessage("eggman_train02", "Fast");
					Game.ProcessMessage("common_object_event02", "GateOpen");
					Game.ProcessMessage("common_object_event05", "GateClose");
					Game.ProcessMessage("common_object_event06", "GateClose");
					Game.ProcessMessage("eggman_train02", "Horn");
					Game.ProcessMessage("common_object_event09", "GateClose");
					Game.ProcessMessage("common_object_event10", "GateClose");
					Game.StartEntityByName("Barrier02Close");
				}
				if (Function == "train_brake02")
				{
					Game.ProcessMessage("eggman_train02", "Stop");
					Game.ProcessMessage("eggman_train02", "Horn");
				}
				if (Function == "door_open04")
				{
					Game.ProcessMessage("eggman_train02", "Go");
					Game.ProcessMessage("eggman_train02", "Fast");
					Game.ProcessMessage("rct_door02", "GateOpen");
					Game.ProcessMessage("common_laser07", "GateOpen");
					Game.ProcessMessage("common_laser08", "GateOpen");
					Game.ProcessMessage("common_laser05", "GateClose");
					Game.ProcessMessage("common_laser06", "GateClose");
					Game.ProcessMessage("common_object_event02", "GateClose");
					Game.ProcessMessage("common_object_event05", "GateClose");
					Game.ProcessMessage("common_object_event06", "GateClose");
					Game.StartEntityByName("Barrier02Open");
				}
				if (Function == "train_horn03")
				{
					Game.ProcessMessage("eggman_train02", "Horn");
				}
				if (Function == "train_horn04")
				{
					Game.ProcessMessage("eggman_train02", "Horn");
				}
				if (Function == "train_camera02")
				{
					Game.ProcessMessage("eggman_train02", "Camera");
					Game.ProcessMessage("eggman_train02", "Fast");
					Game.ProcessMessage("eggman_train02", "Horn");
					Game.StartEntityByName("TrainEscape");
				}
				if (Function == "train_bust02")
				{
					Game.ProcessMessage("eggman_train02", "Bomb");
				}
			}
			if (CharSection == "a_sv")
			{
				if (Function == "cage01")
				{
					Game.Signal("common_cage01");
				}
				if (Function == "cage02")
				{
					Game.Signal("common_cage02");
				}
				if (Function == "cage03")
				{
					Game.Signal("common_cage03");
				}
				if (Function == "cage04")
				{
					Game.Signal("common_cage04");
				}
				if (Function == "cage05")
				{
					Game.Signal("common_cage05");
				}
				if (Function == "train_start04")
				{
					Game.ProcessMessage("freight_train03", "Go");
					Game.ProcessMessage("freight_train03", "Horn");
					Game.ProcessMessage("freight_train06", "Go");
					Game.ProcessMessage("freight_train06", "Horn");
				}
				if (Function == "train_start05")
				{
					Game.ProcessMessage("freight_train04", "Go");
					Game.ProcessMessage("freight_train05", "Go");
					Game.ProcessMessage("freight_train04", "Horn");
					Game.ProcessMessage("freight_train05", "Horn");
				}
				if (Function == "train_start06")
				{
					Game.ProcessMessage("freight_train01", "Go");
					Game.ProcessMessage("freight_train02", "Go");
					Game.ProcessMessage("freight_train01", "Horn");
					Game.ProcessMessage("freight_train02", "Horn");
				}
				if (Function == "train_stop")
				{
					Game.ProcessMessage("freight_train01", "Stop");
					Game.ProcessMessage("freight_train02", "Stop");
					Game.ProcessMessage("freight_train03", "Stop");
					Game.ProcessMessage("freight_train04", "Stop");
					Game.ProcessMessage("freight_train05", "Stop");
					Game.ProcessMessage("freight_train06", "Stop");
				}
			}
			if (CharSection == "b_sn")
			{
				if (Function == "train_start01")
				{
					Game.ProcessMessage("eggman_train01", "Go");
					Game.ProcessMessage("eggman_train01", "Horn");
				}
				if (Function == "train_start02")
				{
					Game.ProcessMessage("eggman_train02", "Go");
					Game.ProcessMessage("eggman_train02", "Horn");
				}
				if (Function == "train_horn01")
				{
					Game.ProcessMessage("eggman_train02", "Horn");
				}
				if (Function == "train_cut01")
				{
					Game.ProcessMessage("eggman_train02", "Cut");
				}
				if (Function == "train_cut02")
				{
					Game.ProcessMessage("eggman_train02", "Cut");
				}
				if (Function == "train_cut03")
				{
					Game.ProcessMessage("eggman_train02", "Cut");
				}
				if (Function == "train_cut04")
				{
					Game.ProcessMessage("eggman_train02", "Cut");
				}
				if (Function == "train_start05")
				{
					Game.ProcessMessage("freight_train04", "Go");
					Game.ProcessMessage("freight_train05", "Go");
					Game.ProcessMessage("freight_train04", "Horn");
					Game.ProcessMessage("freight_train05", "Horn");
				}
				if (Function == "train_stop01")
				{
					Game.ProcessMessage("freight_train04", "Stop");
					Game.ProcessMessage("freight_train05", "Stop");
				}
				if (Function == "train_start04")
				{
					Game.ProcessMessage("freight_train02", "Go");
					Game.ProcessMessage("freight_train03", "Go");
					Game.ProcessMessage("freight_train02", "Horn");
					Game.ProcessMessage("freight_train03", "Horn");
				}
				if (Function == "train_stop02")
				{
					Game.ProcessMessage("freight_train02", "Stop");
					Game.ProcessMessage("freight_train03", "Stop");
				}
			}
			if (CharSection == "b_sd")
			{
				if (Function == "train_start01")
				{
					Game.ProcessMessage("eggman_train01", "Go");
					Game.ProcessMessage("eggman_train01", "Horn");
				}
				if (Function == "train_start02")
				{
					Game.ProcessMessage("eggman_train02", "Go");
					Game.ProcessMessage("eggman_train02", "Horn");
				}
				if (Function == "train_horn01")
				{
					Game.ProcessMessage("eggman_train02", "Horn");
				}
				if (Function == "train_camera01")
				{
					Game.ProcessMessage("eggman_train02", "Camera");
					Game.ProcessMessage("eggman_train02", "Fast");
					Game.ProcessMessage("eggman_train02", "Horn");
					Game.StartEntityByName("TrainEscape");
				}
				if (Function == "train_bust01")
				{
					Game.ProcessMessage("eggman_train02", "Bomb");
				}
				if (Function == "train_start05")
				{
					Game.ProcessMessage("freight_train04", "Go");
					Game.ProcessMessage("freight_train05", "Go");
					Game.ProcessMessage("freight_train04", "Horn");
					Game.ProcessMessage("freight_train05", "Horn");
				}
				if (Function == "train_start04")
				{
					Game.ProcessMessage("freight_train01", "Go");
					Game.ProcessMessage("freight_train02", "Go");
					Game.ProcessMessage("freight_train03", "Go");
					Game.ProcessMessage("freight_train01", "Horn");
					Game.ProcessMessage("freight_train02", "Horn");
					Game.ProcessMessage("freight_train03", "Horn");
				}
				if (Function == "train_stop")
				{
					Game.ProcessMessage("freight_train01", "Stop");
					Game.ProcessMessage("freight_train02", "Stop");
					Game.ProcessMessage("freight_train03", "Stop");
					Game.ProcessMessage("freight_train04", "Stop");
					Game.ProcessMessage("freight_train05", "Stop");
				}
				if (Function == "eggman_train_destroy")
				{
					Game.Result();
				}
			}
		}
		if (Stage == "tpj")
		{
			if (CharSection == "a_sn" && Function == "AtoB")
			{
				Game.ChangeArea("tpj_b_sn");
			}
			if (CharSection == "b_sn")
			{
				if (Function == "AtoB")
				{
					Game.ChangeArea("tpj_b_sn");
				}
				if (Function == "cage01")
				{
					Game.Signal("common_cage01");
				}
			}
			if (CharSection == "c_rg")
			{
				if (Function == "layser_off")
				{
					Game.ProcessMessage("common_laser01", "GateOpen");
					Game.ProcessMessage("common_laser02", "GateOpen");
					Game.ProcessMessage("common_laser03", "GateOpen");
					Game.ProcessMessage("common_laser04", "GateOpen");
					Game.ProcessMessage("common_laser05", "GateOpen");
					Game.ProcessMessage("common_laser06", "GateOpen");
					Game.Signal("LaserWallDestroy");
				}
				if (Function == "open_cage01")
				{
					Game.Signal("common_cage1000");
				}
			}
			if (CharSection == "c_sv")
			{
				if (Function == "Goal_open")
				{
					Game.Signal("common_cage01");
					Game.ProcessMessage("common_laser07", "GateOpen");
					Game.ProcessMessage("common_laser08", "GateOpen");
					Game.ProcessMessage("common_laser09", "GateOpen");
					Game.ProcessMessage("common_laser10", "GateOpen");
					Game.ProcessMessage("common_laser11", "GateOpen");
					Game.ProcessMessage("common_laser12", "GateOpen");
					Game.Signal("LaserWallDestroy");
				}
				if (Function == "Oneup")
				{
					Game.Signal("common_cage02");
				}
			}
		}
		if (Stage == "kdv")
		{
			if (CharSection == "a_sn")
			{
				if (Function == "goto_c")
				{
					Game.ChangeArea("kdv_d_sn");
				}
				if (Function == "brake01")
				{
					Game.Signal("inclinedbridge01");
				}
				if (Function == "brake02")
				{
					Game.Signal("inclinedbridge03");
				}
				if (Function == "brake03")
				{
					Game.Signal("inclinedbridge02");
				}
			}
			if (CharSection == "a_sd")
			{
				if (Function == "goto_d")
				{
					Game.ChangeArea("kdv_d_sd");
				}
				if (Function == "brake01")
				{
					Game.Signal("inclinedbridge01");
				}
				if (Function == "brake02")
				{
					Game.Signal("inclinedbridge03");
				}
				if (Function == "brake03")
				{
					Game.Signal("inclinedbridge02");
				}
			}
			if (CharSection == "d_sn")
			{
				if (Function == "goto_b")
				{
					Game.ChangeArea("kdv_b_sn");
				}
				if (Function == "esp002")
				{
					Game.Signal("espstairs_left01");
					Game.ProcessMessage("espstairs_left01", "PsiEffect", true);
				}
				if (Function == "door01")
				{
					Game.Signal("kdv_door01");
				}
				if (Function == "door02")
				{
					Game.Signal("kdv_door02");
				}
				if (Function == "esp001")
				{
					Game.Signal("physicspath01");
					Game.ProcessMessage("physicspath01", "PsiEffect", true);
					Game.Signal("physicspath02");
					Game.ProcessMessage("physicspath02", "PsiEffect", true);
					Game.Signal("physicspath03");
					Game.ProcessMessage("physicspath03", "PsiEffect", true);
					Game.Signal("physicspath04");
					Game.ProcessMessage("physicspath04", "PsiEffect", true);
					Game.Signal("physicspath05");
					Game.ProcessMessage("physicspath05", "PsiEffect", true);
				}
				if (Function == "door03")
				{
					Game.Signal("kdv_door04");
				}
			}
			if (CharSection == "d_sd")
			{
				if (Function == "goto_b")
				{
					Game.ChangeArea("kdv_b_sd");
				}
				if (Function == "cage01")
				{
					Game.Signal("common_cage02");
					Game.Signal("kdv_door05");
				}
				if (Function == "door01")
				{
					Game.Signal("kdv_door01");
					Game.StartEntityByName("amigo_collision01");
				}
				if (Function == "door02")
				{
					Game.Signal("kdv_door02");
				}
				if (Function == "door03")
				{
					Game.Signal("kdv_door03");
					Game.Signal("kdv_door04");
				}
				if (Function == "laser02")
				{
					Game.ProcessMessage("common_laser11", "GateOpen");
					Game.ProcessMessage("common_laser12", "GateOpen");
				}
				if (Function == "laser01")
				{
					Game.ProcessMessage("common_laser02", "GateOpen");
					Game.ProcessMessage("common_laser01", "GateOpen");
				}
				if (Function == "cage02")
				{
					Game.Signal("common_cage03");
				}
				if (Function == "close01")
				{
					Game.ProcessMessage("kdv_door01", "GateClose");
					Game.StartEntityByName("close01_collision");
				}
			}
			if (CharSection == "d_sv")
			{
				if (Function == "goto_b")
				{
					Game.ChangeArea("kdv_b_sv");
				}
				if (Function == "esp001")
				{
					Game.Signal("espstairs_right01");
					Game.ProcessMessage("espstairs_right01", "PsiEffect", true);
				}
				if (Function == "esp002")
				{
					Game.Signal("espstairs_left01");
					Game.ProcessMessage("espstairs_left01", "PsiEffect", true);
				}
				if (Function == "iwa001")
				{
					Game.Signal("brickwall02");
				}
				if (Function == "iwa002")
				{
					Game.Signal("brickwall03");
				}
				if (Function == "open_cage01")
				{
					Game.Signal("common_cage01");
				}
				if (Function == "door01")
				{
					Game.Signal("kdv_door01");
				}
				if (Function == "door02")
				{
					Game.Signal("kdv_door02");
					Game.ProcessMessage("common_laser1000", "GateOpen");
					Game.ProcessMessage("common_laser1001", "GateOpen");
					Game.ProcessMessage("common_laser1002", "GateOpen");
					Game.ProcessMessage("common_laser1003", "GateOpen");
					Game.ProcessMessage("common_laser1004", "GateOpen");
					Game.ProcessMessage("common_laser1005", "GateOpen");
					Game.StartEntityByName("ColGroup");
				}
				if (Function == "door03")
				{
					Game.Signal("kdv_door03");
				}
				if (Function == "door04")
				{
					Game.Signal("kdv_door04");
				}
				if (Function == "inclinedbridge1000")
				{
					Game.Signal("inclinedbridge01");
					Game.StartEntityByName("hintcollision1005");
				}
			}
			if (CharSection == "b_sn")
			{
				if (Function == "goto_c")
				{
					Game.ChangeArea("kdv_c_sn");
				}
				if (Function == "brake01")
				{
					Game.Signal("objectphysics43");
					Game.Signal("impulsesphere01");
				}
				if (Function == "brake02")
				{
					Game.Signal("objectphysics52");
					Game.Signal("impulsesphere02");
				}
				if (Function == "brake03")
				{
					Game.Signal("objectphysics03");
					Game.Signal("objectphysics02");
				}
				if (Function == "brake04")
				{
					Game.Signal("objectphysics87");
					Game.Signal("objectphysics88");
				}
				if (Function == "brake05")
				{
					Game.Signal("objectphysics89");
					Game.Signal("objectphysics90");
				}
				if (Function == "brake06")
				{
					Game.Signal("inclinedbridge01");
				}
				if (Function == "door01")
				{
					Game.Signal("kdv_door01");
				}
				if (Function == "door02")
				{
					Game.Signal("kdv_door02");
				}
				if (Function == "door03")
				{
					Game.Signal("kdv_door03");
				}
				if (Function == "door04")
				{
					Game.Signal("kdv_door04");
				}
				if (Function == "door05")
				{
					Game.Signal("kdv_door05");
				}
				if (Function == "brake05_brk")
				{
					Game.Signal("objectphysics51");
					Game.Signal("impulsesphere03");
				}
			}
			if (CharSection == "b_sd")
			{
				if (Function == "brake01")
				{
					Game.Signal("objectphysics52");
					Game.Signal("impulsesphere01");
				}
				if (Function == "brake02")
				{
					Game.Signal("objectphysics55");
					Game.Signal("impulsesphere02");
				}
				if (Function == "brake06")
				{
					Game.Signal("inclinedbridge01");
				}
				if (Function == "open_cage")
				{
					Game.Signal("common_cage01");
				}
				if (Function == "open_cage02")
				{
					Game.Signal("common_cage02");
				}
				if (Function == "door03")
				{
					Game.Signal("kdv_door03");
				}
				if (Function == "door02")
				{
					Game.Signal("kdv_door02");
				}
				if (Function == "door05")
				{
					Game.Signal("kdv_door01");
				}
				if (Function == "brake03")
				{
					Game.Signal("objectphysics51");
					Game.Signal("impulsesphere03");
				}
				if (Function == "door_close/kdv_door05")
				{
					Game.ProcessMessage("kdv_door05", "GateClose");
				}
			}
			if (CharSection == "b_sv")
			{
				if (Function == "brake02")
				{
					Game.Signal("objectphysics52");
					Game.Signal("impulsesphere02");
				}
				if (Function == "door01")
				{
					Game.Signal("kdv_door01");
				}
				if (Function == "door03")
				{
					Game.Signal("kdv_door03");
				}
				if (Function == "espstairs01")
				{
					Game.Signal("espstairs_right01");
					Game.ProcessMessage("espstairs_right01", "PsiEffect", true);
				}
				if (Function == "cage1000")
				{
					Game.Signal("common_cage1000");
				}
				if (Function == "break1000")
				{
					Game.Signal("objectphysics51");
					Game.Signal("impulsesphere1000");
				}
				if (Function == "kdv_all_destroy")
				{
					Game.Result();
				}
			}
			if (CharSection == "c_sn" && Function == "door1000")
			{
				Game.ProcessMessage("kdv_door1000", "GateClose");
			}
			if (CharSection == "e_sn")
			{
				if (Function == "brake01")
				{
					Game.Signal("inclinedbridge01");
				}
				if (Function == "brake02")
				{
					Game.Signal("inclinedbridge03");
				}
				if (Function == "brake03")
				{
					Game.Signal("inclinedbridge02");
				}
			}
		}
		if (!(Stage == "aqa"))
		{
			return;
		}
		if (CharSection == "a_sn")
		{
			if (Function == "door01")
			{
				Game.Signal("aqa_door01");
			}
			if (Function == "door02")
			{
				Game.Signal("aqa_door02");
			}
			if (Function == "door03")
			{
				Game.Signal("aqa_door03");
			}
			if (Function == "door04")
			{
				Game.Signal("aqa_door04");
			}
			if (Function == "door05")
			{
				Game.Signal("aqa_door05");
			}
			if (Function == "laser01")
			{
				Game.ProcessMessage("common_laser60", "GateOpen");
				Game.ProcessMessage("common_laser61", "GateOpen");
			}
			if (Function == "door06")
			{
				Game.Signal("aqa_door06");
			}
			if (Function == "door07")
			{
				Game.Signal("aqa_door08");
			}
			if (Function == "door08")
			{
				Game.Signal("aqa_door11");
				Game.ProcessMessage("common_laser68", "GateOpen");
				Game.ProcessMessage("common_laser69", "GateOpen");
				Game.ProcessMessage("common_laser1006", "GateOpen");
				Game.ProcessMessage("common_laser1007", "GateOpen");
			}
			if (Function == "door09")
			{
				Game.ProcessMessage("aqa_door13", "GateClose");
			}
			if (Function == "enemy01")
			{
				Game.StartEntityByName("GroupHelper11");
				Game.ProcessMessage("aqa_door10", "GateClose");
				Game.ProcessMessage("aqa_door13", "GateOpen");
			}
			if (Function == "door10")
			{
				Game.Signal("aqa_door15");
				Game.StartEntityByName("common_hint_collision06_group");
			}
			if (Function == "door11")
			{
				Game.Signal("aqa_door10");
			}
			if (Function == "door13")
			{
				Game.Signal("aqa_door16");
			}
			if (Function == "door15")
			{
				Game.Signal("aqa_door19");
			}
			if (Function == "door16")
			{
				Game.ProcessMessage("aqa_door08", "GateClose");
			}
			if (Function == "door17")
			{
				Game.ProcessMessage("aqa_door14", "GateOpen");
			}
			if (Function == "door12")
			{
				Game.Signal("aqa_door12");
			}
			if (Function == "amigo06")
			{
				Game.StartEntityByName("amigo_collision03_group");
			}
			if (Function == "tails01")
			{
				Game.StartEntityByName("TailsGroup");
			}
			if (Function == "cage01")
			{
				Game.Signal("common_cage01");
			}
			if (Function == "cage02")
			{
				Game.Signal("common_cage02");
			}
			if (Function == "goto_b")
			{
				Game.ChangeArea("aqa_b_sn");
			}
		}
		if (CharSection == "a_sd")
		{
			if (Function == "door01")
			{
				Game.Signal("aqa_door18");
			}
			if (Function == "door02")
			{
				Game.Signal("aqa_door09");
			}
			if (Function == "door09")
			{
				Game.Signal("aqa_door13");
				Game.StartEntityByName("GroupHelper102");
			}
			if (Function == "door10")
			{
				Game.Signal("aqa_door12");
			}
			if (Function == "door03")
			{
				Game.Signal("common_cage01");
				Game.Signal("aqa_door10");
				Game.StartEntityByName("ArenaCameraDestroy");
			}
			if (Function == "door11")
			{
				Game.Signal("aqa_door14");
				Game.Signal("aqa_door15");
				Game.ProcessMessage("aqa_door09", "GateClose");
				Game.ProcessMessage("aqa_door10", "GateClose");
				Game.StartEntityByName("GroupHelper100");
			}
			if (Function == "door08")
			{
				Game.Signal("aqa_door11");
				Game.StartEntityByName("GroupHelper40");
				Game.ProcessMessage("common_laser68", "GateOpen");
				Game.ProcessMessage("common_laser69", "GateOpen");
				Game.ProcessMessage("common_laser1006", "GateOpen");
				Game.ProcessMessage("common_laser1007", "GateOpen");
			}
			if (Function == "laser01")
			{
				Game.ProcessMessage("common_laser60", "GateOpen");
				Game.ProcessMessage("common_laser61", "GateOpen");
			}
			if (Function == "kakushi01")
			{
				Game.Signal("aqa_door16");
			}
			if (Function == "door_open/aqa_door11")
			{
				Game.ProcessMessage("aqa_door11", "GateClose");
			}
			if (Function == "goto_b")
			{
				Game.ChangeArea("aqa_b_sd");
			}
		}
		if (CharSection == "a_sv")
		{
			if (Function == "door01")
			{
				Game.Signal("aqa_door18");
			}
			if (Function == "door02")
			{
				Game.Signal("aqa_door09");
			}
			if (Function == "door03")
			{
				Game.Signal("common_cage01");
				Game.Signal("aqa_door10");
				Game.StartEntityByName("ArenaCameraDestroy");
			}
			if (Function == "laser02")
			{
				Game.ProcessMessage("common_laser60", "GateOpen");
				Game.ProcessMessage("common_laser61", "GateOpen");
			}
			if (Function == "door08")
			{
				Game.Signal("aqa_door11");
				Game.ProcessMessage("common_laser68", "GateOpen");
				Game.ProcessMessage("common_laser69", "GateOpen");
				Game.ProcessMessage("common_laser1006", "GateOpen");
				Game.ProcessMessage("common_laser1007", "GateOpen");
				Game.StartEntityByName("GroupHelper1002");
			}
			if (Function == "esp02")
			{
				Game.Signal("aqa_door12");
			}
			if (Function == "door09")
			{
				Game.Signal("aqa_door13");
				Game.StartEntityByName("GroupHelper1001");
			}
			if (Function == "door10")
			{
				Game.Signal("aqa_door21");
			}
			if (Function == "door04")
			{
				Game.Signal("aqa_door08");
				Game.Signal("aqa_door14");
				Game.ProcessMessage("aqa_door09", "GateClose");
				Game.StartEntityByName("GroupHelper1003");
			}
			if (Function == "door11")
			{
				Game.Signal("aqa_door20");
			}
			if (Function == "door12")
			{
				Game.Signal("aqa_door25");
			}
			if (Function == "goto_b")
			{
				Game.ChangeArea("aqa_b_sv");
			}
			if (Function == "door_close/aqa_door10")
			{
				Game.ProcessMessage("aqa_door10", "GateClose");
				Game.StartEntityByName("GroupHelper1000");
			}
			if (Function == "signal/common_cage02")
			{
				Game.Signal("common_cage02");
			}
			if (Function == "finallaser")
			{
				Game.ProcessMessage("common_laser1008", "GateOpen");
				Game.ProcessMessage("common_laser1009", "GateOpen");
			}
			if (Function == "flameup_door01")
			{
				Game.Signal("aqa_door16");
				Game.ProcessMessage("aqa_door16", "PsiEffect", true);
			}
			if (Function == "flameup_door02")
			{
				Game.StartEntityByName("UpgradeGroupHelper1000");
			}
			if (Function == "flameup_laser01")
			{
				Game.ProcessMessage("common_laser1100", "GateOpen");
				Game.ProcessMessage("common_laser1101", "GateOpen");
				Game.ProcessMessage("common_laser1102", "GateOpen");
				Game.ProcessMessage("common_laser1103", "GateOpen");
			}
			if (Function == "flameup_cage01")
			{
				Game.Signal("common_cage1000");
			}
			if (Function == "new_door15")
			{
				Game.Signal("aqa_door15");
				Game.Signal("aqa_lamp1000");
				Game.Signal("aqa_lamp1001");
			}
		}
		if (CharSection == "b_sn")
		{
			if (Function == "door01")
			{
				Game.Signal("aqa_door01");
			}
			if (Function == "door02")
			{
				Game.Signal("aqa_door02");
			}
			if (Function == "door_close02")
			{
				Game.ProcessMessage("aqa_door02", "GateClose");
			}
			if (Function == "door03")
			{
				Game.Signal("aqa_door03");
			}
			if (Function == "door04")
			{
				Game.Signal("aqa_door04");
			}
			if (Function == "door07")
			{
				Game.Signal("aqa_door07");
			}
			if (Function == "door06")
			{
				Game.StartEntityByName("amigo_collision01");
				Game.Signal("obj_destroyer01");
			}
			if (Function == "door08")
			{
				Game.Signal("aqa_door08");
			}
			if (Function == "door09")
			{
				Game.Signal("aqa_door09");
			}
			if (Function == "door10")
			{
				Game.Signal("aqa_door10");
			}
			if (Function == "close01")
			{
				Game.Signal("aqa_glass_blue01");
			}
			if (Function == "close02")
			{
				Game.Signal("aqa_glass_blue02");
			}
			if (Function == "close03")
			{
				Game.Signal("aqa_glass_blue03");
			}
			if (Function == "close04")
			{
				Game.Signal("aqa_glass_blue04");
			}
			if (Function == "close05")
			{
				Game.Signal("aqa_glass_red01");
			}
			if (Function == "close06")
			{
				Game.Signal("aqa_glass_red02");
			}
			if (Function == "close07")
			{
				Game.Signal("aqa_glass_red03");
			}
			if (Function == "close08")
			{
				Game.Signal("aqa_glass_red04");
			}
			if (Function == "close09")
			{
				Game.Signal("aqa_glass_red05");
			}
			if (Function == "close10")
			{
				Game.Signal("aqa_glass_red06");
			}
			if (Function == "close11")
			{
				Game.Signal("aqa_glass_red07");
			}
			if (Function == "close12")
			{
				Game.Signal("aqa_glass_red08");
			}
			if (Function == "area00")
			{
				Game.StartEntityByName("area00_group");
			}
			if (Function == "area01")
			{
				Game.ProcessMessage("common_laser01", "GateOpen");
				Game.ProcessMessage("common_laser02", "GateOpen");
				Game.ProcessMessage("common_laser03", "GateOpen");
				Game.ProcessMessage("common_laser04", "GateOpen");
				Game.ProcessMessage("common_laser05", "GateOpen");
				Game.ProcessMessage("common_laser06", "GateOpen");
			}
			if (Function == "area02")
			{
				Game.Signal("aqa_door06");
			}
		}
		if (CharSection == "b_sd")
		{
			if (Function == "door02")
			{
				Game.Signal("aqa_door05");
			}
			if (Function == "door04")
			{
				Game.Signal("aqa_door04");
			}
			if (Function == "door07")
			{
				Game.Signal("aqa_door07");
			}
			if (Function == "door01")
			{
				Game.Signal("aqa_door01");
			}
			if (Function == "door03")
			{
				Game.Signal("aqa_door06");
			}
			if (Function == "close01")
			{
				Game.Signal("aqa_glass_blue01");
			}
			if (Function == "close02")
			{
				Game.Signal("aqa_glass_blue02");
			}
			if (Function == "close03")
			{
				Game.Signal("aqa_glass_blue03");
			}
			if (Function == "close04")
			{
				Game.Signal("aqa_glass_blue04");
			}
			if (Function == "door_last")
			{
				Game.Signal("aqa_door08");
				Game.StartEntityByName("ArenaCameraDestroy");
			}
		}
		if (CharSection == "b_sv")
		{
			if (Function == "door01")
			{
				Game.Signal("aqa_door06");
			}
			if (Function == "door02")
			{
				Game.Signal("aqa_door05");
			}
			if (Function == "door03")
			{
				Game.Signal("aqa_door07");
			}
			if (Function == "door04")
			{
				Game.Signal("aqa_door02");
			}
			if (Function == "door05")
			{
				Game.Signal("aqa_door08");
			}
			if (Function == "laser08")
			{
				Game.ProcessMessage("common_laser07", "GateOpen");
				Game.ProcessMessage("common_laser08", "GateOpen");
			}
			if (Function == "door1000")
			{
				Game.Signal("aqa_door1000");
			}
		}
	}
}
