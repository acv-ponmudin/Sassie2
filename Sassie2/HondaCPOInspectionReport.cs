﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace Sassie2
{
    internal class HondaCPOInspectionReport
    {
        private DataSet _dsCPOData;
        private string _sAID;

        public DataView[] _dvCPOData;
        public int _NoPostSaleVehicles = 0;
        public int _NoPreSaleVehicles = 0;
        public string _strImagePath = "";
        public string _sCurrentLanguage;
        public string _DivisionCode;
        public string _ReportTitle;

        Dictionary<string, Dictionary<string, string>> presale_vehicles = new Dictionary<string, Dictionary<string, string>>();
        Dictionary<string, Dictionary<string, string>> postsale_vehicles = new Dictionary<string, Dictionary<string, string>>();

        Dictionary<string, string> result_dict = new Dictionary<string, string>();

        List<Dictionary<int, string>> presale_list = new List<Dictionary<int, string>>();
        List<Dictionary<int, string>> postsale_list = new List<Dictionary<int, string>>();

        Dictionary<int, string> vehicle_detail = new Dictionary<int, string>() {
            { 999000,"VIN" },
            { 999001,"Manufacturer" },
            { 999002,"Make_Description" },
            { 999003,"Model_Description" } ,
            { 999004,"Vehicle_Year" },
            { 999005,"Stock_Number" },
            { 999006,"Tier" },
        };

        Dictionary<int, string> presale_mappingA = new Dictionary<int, string>()
        {
            { 0,"question_291" },
            { 999000,"question_301" },
            { 999001,"question_311" },
            { 999002,"question_321" },
            { 999003,"question_341" } ,
            { 999004,"question_351" },
            { 999005,"question_361" },
            { 999006,"question_24041" },
            {22101, "question_431"},
            {22102, "question_451"},
            {22201,"question_501"},
            {22202,"question_531"},
            {22305,"question_571"},
            {22301,"question_601"},
            {22306,"question_631"},
            {22307,"question_661"},
            {22308,"question_691"},
            {22401,"question_731"},
            {22402,"question_761"},
            {22501,"question_1011"},
            {22502,"question_801 "},
            {22503,"question_831"},
            {22504,"question_861"},
            {22505,"question_891"},
            {22506,"question_921"},
            {22507,"question_951"},
            {22508,"question_981"},
            {22601,"question_1221"},
            {22701,"question_1261"},
            {22709,"question_1291"},
            {22703,"question_1321"},
            {22708,"question_1351"},
            {22705,"question_1381"},
            {22706,"question_1411"},
            {22801,"question_1451"},
            {22802,"question_1481"},
            {22804,"question_1541"},
            {22806,"question_1601"},
            {22911,"question_1641"},
            {22902,"question_1671"},
            {22903,"question_1701"},
            {22904,"question_1731"},
            {22905,"question_1761"},
            {22906,"question_1791"},
            {22907,"question_1821"},
            {22912,"question_1851"},
            {22913,"question_1991"},
            {22914,"question_1881"},
            {22807,"question_1511"},
            {22808,"question_1571"}
        };

        Dictionary<int, string> presale_mappingB = new Dictionary<int, string>()
        {
            { 1,"question_20031" },
            { 999000,"question_7321" },
            { 999001,"question_7331" },
            { 999002,"question_7341" },
            { 999003,"question_7351" } ,
            { 999004,"question_7361" },
            { 999005,"question_7371" },
            { 999006,"question_24051" },
            {22101,"question_7381"},
            {22102,"question_7411"},
            {22201,"question_7451"},
            {22202,"question_7481"},
            {22305,"question_7521"},
            {22301,"question_7561"},
            {22306,"question_7601"},
            {22307,"question_7641"},
            {22308,"question_7681"},
            {22401,"question_7721"},
            {22402,"question_7751"},
            {22501,"question_7791"},
            {22502,"question_7821"},
            {22503,"question_7851"},
            {22504,"question_7881"},
            {22505,"question_7911"},
            {22506,"question_7941"},
            {22507,"question_7971"},
            {22508,"question_8001"},
            {22601,"question_8031"},
            {22701,"question_8071"},
            {22709,"question_8101"},
            {22703,"question_8131"},
            {22708,"question_8161"},
            {22705,"question_8191"},
            {22706,"question_8221"},
            {22801,"question_8251"},
            {22802,"question_8281"},
            {22804,"question_8341"},
            {22806,"question_8401"},
            {22911,"question_8431"},
            {22902,"question_8461"},
            {22903,"question_8491"},
            {22904,"question_8521"},
            {22905,"question_8551"},
            {22906,"question_8581"},
            {22907,"question_8611"},
            {22912,"question_8641"},
            {22913,"question_8671"},
            {22914,"question_8701"},
            {22807,"question_8311"},
            {22808,"question_8371"}
        };

        Dictionary<int, string> presale_mappingC = new Dictionary<int, string>()
        {
            { 2,"question_20041" },
            { 999000,"question_5821" },
            { 999001,"question_5831" },
            { 999002,"question_5841" },
            { 999003,"question_5851" } ,
            { 999004,"question_5861" },
            { 999005,"question_5871" },
            { 999006,"question_24061" },
            {22101, "question_5881"},
            {22102, "question_5911"},
            {22201, "question_5951"},
            {22202, "question_5981"},
            {22301, "question_6061"},
            {22305, "question_6021"},
            {22306, "question_6101"},
            {22307, "question_6141"},
            {22308, "question_6181"},
            {22401, "question_6221"},
            {22402, "question_6251"},
            {22501, "question_6291"},
            {22502, "question_6321"},
            {22503, "question_6351"},
            {22504, "question_6381"},
            {22505, "question_6411"},
            {22506, "question_6441"},
            {22507, "question_6471"},
            {22508, "question_6501"},
            {22601, "question_6531"},
            {22701, "question_6571"},
            {22709, "question_6601"},
            {22703, "question_6631"},
            {22708, "question_6661"},
            {22705, "question_6691"},
            {22706, "question_6721"},
            {22801, "question_6751"},
            {22802, "question_6781"},
            {22804, "question_6841"},
            {22806, "question_6901"},
            {22911, "question_6931"},
            {22902, "question_6961"},
            {22903, "question_6991"},
            {22904, "question_7021"},
            {22905, "question_7051"},
            {22906, "question_7081"},
            {22907, "question_7111"},
            {22912, "question_7141"},
            {22913, "question_7171"},
            {22914, "question_7201"},
            {22807, "question_6811"},
            {22808, "question_6871"}
        };

        Dictionary<int, string> presale_mappingD = new Dictionary<int, string>()
        {
            { 3,"question_4311" },
            { 999000,"question_2801" },
            { 999001,"question_2811" },
            { 999002,"question_2821" },
            { 999003,"question_2831" } ,
            { 999004,"question_2841" },
            { 999005,"question_2851" },
            { 999006,"question_24071" },
            { 22101, "question_2861"},
            { 22102, "question_2891"},
            { 22201, "question_2931"},
            { 22202, "question_2961"},
            { 22305, "question_3001"},
            { 22301, "question_3041"},
            { 22306, "question_3081"},
            { 22307, "question_3121"},
            { 22308, "question_3161"},
            { 22401, "question_3201"},
            { 22402, "question_3231"},
            { 22501, "question_3271"},
            { 22502, "question_3301"},
            { 22503, "question_3331"},
            { 22504, "question_3361"},
            { 22505, "question_3391"},
            { 22506, "question_3421"},
            { 22507, "question_3451"},
            { 22508, "question_3481"},
            { 22601, "question_3511"},
            { 22701, "question_3551"},
            { 22709, "question_3581"},
            { 22703, "question_3611"},
            { 22708, "question_3641"},
            { 22705, "question_3671"},
            { 22706, "question_3701"},
            { 22801, "question_3731"},
            { 22802, "question_3761"},
            { 22804, "question_3821"},
            { 22806, "question_3881"},
            { 22911, "question_3911"},
            { 22902, "question_3941"},
            { 22903, "question_3971"},
            { 22904, "question_4001"},
            { 22905, "question_4031"},
            { 22906, "question_4061"},
            { 22907, "question_4091"},
            { 22912, "question_4121"},
            { 22913, "question_4151"},
            { 22914, "question_4181"},
            { 22807, "question_3791"},
            { 22808, "question_3851"}
        };

        Dictionary<int, string> presale_mappingE = new Dictionary<int, string>() {
            { 4,"question_20051" },
            { 999000,"question_14821" },
            { 999001,"question_14831" },
            { 999002,"question_14841" },
            { 999003,"question_14851" } ,
            { 999004,"question_14861" },
            { 999005,"question_14871" },
            { 999006,"question_24081" },
            {22101, "question_14881"},
            {22102, "question_14911"},
            {22201, "question_14951"},
            {22202, "question_14981"},
            {22305, "question_15021"},
            {22301, "question_15061"},
            {22306, "question_15101"},
            {22307, "question_15141"},
            {22308, "question_15181"},
            {22401, "question_15221"},
            {22402, "question_15251"},
            {22501, "question_15291"},
            {22502, "question_15321"},
            {22503, "question_15351"},
            {22504, "question_15381"},
            {22505, "question_15411"},
            {22506, "question_15441"},
            {22507, "question_15471"},
            {22508, "question_15501"},
            {22601, "question_15531"},
            {22701, "question_15571"},
            {22709, "question_15601"},
            {22703, "question_15631"},
            {22708, "question_15661"},
            {22705, "question_15691"},
            {22706, "question_15721"},
            {22801, "question_15751"},
            {22802, "question_15781"},
            {22804, "question_15841"},
            {22806, "question_15901"},
            {22911, "question_15931"},
            {22902, "question_15961"},
            {22903, "question_15991"},
            {22904, "question_16021"},
            {22905, "question_16051"},
            {22906, "question_16081"},
            {22907, "question_16111"},
            {22912, "question_16141"},
            {22913, "question_16171"},
            {22914, "question_16201"},
            {22807, "question_15811"},
            {22808, "question_15871"}
        };

        Dictionary<int, string> presale_mappingF = new Dictionary<int, string>() {
            { 5,"question_20061" },
            { 999000,"question_13321" },
            { 999001,"question_13331" },
            { 999002,"question_13341" },
            { 999003,"question_13351" } ,
            { 999004,"question_13361" },
            { 999005,"question_13371" },
            { 999006,"question_24091" },
            {22101, "question_13381"},
            {22102, "question_13411"},
            {22201, "question_13451"},
            {22202, "question_13481"},
            {22305, "question_13521"},
            {22301, "question_13561"},
            {22306, "question_13601"},
            {22307, "question_13641"},
            {22308, "question_13681"},
            {22401, "question_13721"},
            {22402, "question_13751"},
            {22501, "question_13791"},
            {22502, "question_13821"},
            {22503, "question_13851"},
            {22504, "question_13881"},
            {22505, "question_13911"},
            {22506, "question_13941"},
            {22507, "question_13971"},
            {22508, "question_14001"},
            {22601, "question_14031"},
            {22701, "question_14071"},
            {22709, "question_14101"},
            {22703, "question_14131"},
            {22708, "question_14161"},
            {22705, "question_14191"},
            {22706, "question_14221"},
            {22801, "question_14251"},
            {22802, "question_14281"},
            {22804, "question_14341"},
            {22806, "question_14401"},
            {22911, "question_14431"},
            {22902, "question_14461"},
            {22903, "question_14491"},
            {22904, "question_14521"},
            {22905, "question_14551"},
            {22906, "question_14581"},
            {22907, "question_14611"},
            {22912, "question_14641"},
            {22913, "question_14671"},
            {22914, "question_14701"},
            {22807, "question_14311"},
            {22808, "question_14371"}
        };

        Dictionary<int, string> presale_mappingG = new Dictionary<int, string>() {
            { 6,"question_20071" },
            { 999000,"question_11821" },
            { 999001,"question_11831" },
            { 999002,"question_11841" },
            { 999003,"question_11851" } ,
            { 999004,"question_11861" },
            { 999005,"question_11871" },
            { 999006,"question_24101" },
            {22101, "question_11881"},
            {22102, "question_11911"},
            {22201, "question_11951"},
            {22202, "question_11981"},
            {22305, "question_12021"},
            {22301, "question_12061"},
            {22306, "question_12101"},
            {22307, "question_12141"},
            {22308, "question_12181"},
            {22401, "question_12221"},
            {22402, "question_12251"},
            {22501, "question_12291"},
            {22502, "question_12321"},
            {22503, "question_12351"},
            {22504, "question_12381"},
            {22505, "question_12411"},
            {22506, "question_12441"},
            {22507, "question_12471"},
            {22508, "question_12501"},
            {22601, "question_12531"},
            {22701, "question_12571"},
            {22709, "question_12601"},
            {22703, "question_12631"},
            {22708, "question_12661"},
            {22705, "question_12691"},
            {22706, "question_12721"},
            {22801, "question_12751"},
            {22802, "question_12781"},
            {22804, "question_12841"},
            {22806, "question_12901"},
            {22911, "question_12931"},
            {22902, "question_12961"},
            {22903, "question_12991"},
            {22904, "question_13021"},
            {22905, "question_13051"},
            {22906, "question_13081"},
            {22907, "question_13111"},
            {22912, "question_13141"},
            {22913, "question_13171"},
            {22914, "question_13201"},
            {22807, "question_12811"},
            {22808, "question_12871"}
        };

        Dictionary<int, string> presale_mappingH = new Dictionary<int, string>() {
            { 7,"question_20081" },
            { 999000,"question_10321" },
            { 999001,"question_10331" },
            { 999002,"question_10341" },
            { 999003,"question_10351" } ,
            { 999004,"question_10361" },
            { 999005,"question_10371" },
            { 999006,"question_24111" },
            {22101, "question_10381"},
            {22102, "question_10411"},
            {22201, "question_10451"},
            {22202, "question_10481"},
            {22305, "question_10521"},
            {22301, "question_10561"},
            {22306, "question_10601"},
            {22307, "question_10641"},
            {22308, "question_10681"},
            {22401, "question_10721"},
            {22402, "question_10751"},
            {22501, "question_10791"},
            {22502, "question_10821"},
            {22503, "question_10851"},
            {22504, "question_10881"},
            {22505, "question_10911"},
            {22506, "question_10941"},
            {22507, "question_10971"},
            {22508, "question_11001"},
            {22601, "question_11031"},
            {22701, "question_11071"},
            {22709, "question_11101"},
            {22703, "question_11131"},
            {22708, "question_11161"},
            {22705, "question_11191"},
            {22706, "question_11221"},
            {22801, "question_11251"},
            {22802, "question_11281"},
            {22804, "question_11341"},
            {22806, "question_11401"},
            {22911, "question_11431"},
            {22902, "question_11461"},
            {22903, "question_11491"},
            {22904, "question_11521"},
            {22905, "question_11551"},
            {22906, "question_11581"},
            {22907, "question_11611"},
            {22912, "question_11641"},
            {22913, "question_11671"},
            {22914, "question_11701"},
            {22807, "question_11311"},
            {22808, "question_11371"}
        };

        Dictionary<int, string> presale_mappingI = new Dictionary<int, string>() {
            { 8,"question_20091" },
            { 999000,"question_4321" },
            { 999001,"question_4331" },
            { 999002,"question_4341" },
            { 999003,"question_4351" } ,
            { 999004,"question_4361" },
            { 999005,"question_4371" },
            { 999006,"question_24121" },
            {22101, "question_4381"},
            {22102, "question_4411"},
            {22201, "question_4451"},
            {22202, "question_4481"},
            {22305, "question_4521"},
            {22301, "question_4561"},
            {22306, "question_4601"},
            {22307, "question_4641"},
            {22308, "question_4681"},
            {22401, "question_4721"},
            {22402, "question_4751"},
            {22501, "question_4791"},
            {22502, "question_4821"},
            {22503, "question_4851"},
            {22504, "question_4881"},
            {22505, "question_4911"},
            {22506, "question_4941"},
            {22507, "question_4971"},
            {22508, "question_5001"},
            {22601, "question_5031"},
            {22701, "question_5071"},
            {22709, "question_5101"},
            {22703, "question_5131"},
            {22708, "question_5161"},
            {22705, "question_5191"},
            {22706, "question_5221"},
            {22801, "question_5251"},
            {22802, "question_5281"},
            {22804, "question_5341"},
            {22806, "question_5401"},
            {22911, "question_5431"},
            {22902, "question_5461"},
            {22903, "question_5491"},
            {22904, "question_5521"},
            {22905, "question_5551"},
            {22906, "question_5581"},
            {22907, "question_5611"},
            {22912, "question_5641"},
            {22913, "question_5671"},
            {22914, "question_5701"},
            {22807, "question_5311"},
            {22808, "question_5371"}
        };

        Dictionary<int, string> presale_mappingJ = new Dictionary<int, string>() {
            { 9,"question_20101" },
            { 999000,"question_8821" },
            { 999001,"question_8831" },
            { 999002,"question_8841" },
            { 999003,"question_8851" } ,
            { 999004,"question_8861" },
            { 999005,"question_8871" },
            { 999006,"question_24131" },
            {22101, "question_8881"},
            {22102, "question_8911"},
            {22201, "question_8951"},
            {22202, "question_8981"},
            {22305, "question_9021"},
            {22301, "question_9061"},
            {22306, "question_9101"},
            {22307, "question_9141"},
            {22308, "question_9181"},
            {22401, "question_9221"},
            {22402, "question_9251"},
            {22501, "question_9291"},
            {22502, "question_9321"},
            {22503, "question_9351"},
            {22504, "question_9381"},
            {22505, "question_9411"},
            {22506, "question_9441"},
            {22507, "question_9471"},
            {22508, "question_9501"},
            {22601, "question_9531"},
            {22701, "question_9571"},
            {22709, "question_9601"},
            {22703, "question_9631"},
            {22708, "question_9661"},
            {22705, "question_9691"},
            {22706, "question_9721"},
            {22801, "question_9751"},
            {22802, "question_9781"},
            {22804, "question_9841"},
            {22806, "question_9901"},
            {22911, "question_9931"},
            {22902, "question_9961"},
            {22903, "question_9991"},
            {22904, "question_10021"},
            {22905, "question_10051"},
            {22906, "question_10081"},
            {22907, "question_10111"},
            {22912, "question_10141"},
            {22913, "question_10171"},
            {22914, "question_10201"},
            {22807, "question_9811"},
            {22808, "question_9871"}
        };

        Dictionary<int, string> postsale_mappingA = new Dictionary<int, string>() {
            { 0,"question_2031" },
            { 999000,"question_2041" },
            { 999001,"question_2051" },
            { 999002,"question_2061" },
            { 999003,"question_2081" } ,
            { 999004,"question_2091" },
            { 999005,"question_2101" },
            { 999006,"question_24141" },
            {21101, "question_2141"},
            {21102, "question_2171"},
            {21103, "question_2201"},
            {21104, "question_2231"},
            {21201, "question_2261"},
            {21202, "question_2291"}
        };
        Dictionary<int, string> postsale_mappingB = new Dictionary<int, string>() {
             { 1,"question_16421" },
            { 999000,"question_16431" },
            { 999001,"question_16441" },
            { 999002,"question_16451" },
            { 999003,"question_16461" } ,
            { 999004,"question_16471" },
            { 999005,"question_16481" },
            { 999006,"question_24151" },
            {21101, "question_16511"},
            {21102, "question_16551"},
            {21103, "question_16591"},
            {21104, "question_16631"},
            {21201, "question_16661"},
            {21202, "question_16691"}
        };
        Dictionary<int, string> postsale_mappingC = new Dictionary<int, string>() {
             { 2,"question_16741" },
            { 999000,"question_16751" },
            { 999001,"question_16761" },
            { 999002,"question_16771" },
            { 999003,"question_16781" } ,
            { 999004,"question_16791" },
            { 999005,"question_16801" },
            { 999006,"question_24161" },
            {21101, "question_16831"},
            {21102, "question_16871"},
            {21103, "question_16911"},
            {21104, "question_16951"},
            {21201, "question_16981"},
            {21202, "question_17011"}
        };
        Dictionary<int, string> postsale_mappingD = new Dictionary<int, string>()
        {
             { 3,"question_17061" },
            { 999000,"question_17071" },
            { 999001,"question_17081" },
            { 999002,"question_17091" },
            { 999003,"question_17101" } ,
            { 999004,"question_17111" },
            { 999005,"question_17121" },
            { 999006,"question_24171" },
            {21101, "question_17151"},
            {21102, "question_17191"},
            {21103, "question_17231"},
            {21104, "question_17271"},
            {21201, "question_17301"},
            {21202, "question_17331"}
        };
        Dictionary<int, string> postsale_mappingE = new Dictionary<int, string>()
        {
             { 4,"question_17381" },
            { 999000,"question_17391" },
            { 999001,"question_17401" },
            { 999002,"question_17411" },
            { 999003,"question_17421" } ,
            { 999004,"question_17431" },
            { 999005,"question_17441" },
            { 999006,"question_24181" },
            {21101, "question_17471"},
            {21102, "question_17511"},
            {21103, "question_17551"},
            {21104, "question_17591"},
            {21201, "question_17621"},
            {21202, "question_17651"}
        };
        Dictionary<int, string> postsale_mappingF = new Dictionary<int, string>()
        {
             { 5,"question_17701" },
            { 999000,"question_17711" },
            { 999001,"question_17721" },
            { 999002,"question_17731" },
            { 999003,"question_17741" } ,
            { 999004,"question_17751" },
            { 999005,"question_17761" },
            { 999006,"question_24191" },
            {21101, "question_17791"},
            {21102, "question_17831"},
            {21103, "question_17871"},
            {21104, "question_17911"},
            {21201, "question_17941"},
            {21202, "question_17971"}
        };
        Dictionary<int, string> postsale_mappingG = new Dictionary<int, string>()
        {
             { 6,"question_18021" },
            { 999000,"question_18031" },
            { 999001,"question_18041" },
            { 999002,"question_18051" },
            { 999003,"question_18061" } ,
            { 999004,"question_18071" },
            { 999005,"question_18081" },
            { 999006,"question_24201" },
            {21101, "question_18111"},
            {21102, "question_18151"},
            {21103, "question_18191"},
            {21104, "question_18231"},
            {21201, "question_18261"},
            {21202, "question_18291"}
        };
        Dictionary<int, string> postsale_mappingH = new Dictionary<int, string>()
        {
             { 7,"question_18341" },
            { 999000,"question_18351" },
            { 999001,"question_18361" },
            { 999002,"question_18371" },
            { 999003,"question_18381" } ,
            { 999004,"question_18391" },
            { 999005,"question_18401" },
            { 999006,"question_24211" },
            {21101, "question_18431"},
            {21102, "question_18471"},
            {21103, "question_18511"},
            {21104, "question_18551"},
            {21201, "question_18581"},
            {21202, "question_18611"}
        };
        Dictionary<int, string> postsale_mappingI = new Dictionary<int, string>()
        {
             { 8,"question_18661" },
            { 999000,"question_18671" },
            { 999001,"question_18681" },
            { 999002,"question_18691" },
            { 999003,"question_18701" } ,
            { 999004,"question_18711" },
            { 999005,"question_18721" },
            { 999006,"question_24221" },
            {21101, "question_18751"},
            {21102, "question_18791"},
            {21103, "question_18831"},
            {21104, "question_18871"},
            {21201, "question_18901"},
            {21202, "question_18931"}
        };
        Dictionary<int, string> postsale_mappingJ = new Dictionary<int, string>()
        {
             { 9,"question_18981" },
            { 999000,"question_18991" },
            { 999001,"question_19001" },
            { 999002,"question_19011" },
            { 999003,"question_19021" } ,
            { 999004,"question_19031" },
            { 999005,"question_19041" },
            { 999006,"question_24231" },
            {21101, "question_19071"},
            {21102, "question_19111"},
            {21103, "question_19151"},
            {21104, "question_19191"},
            {21201, "question_19221"},
            {21202, "question_19251"}
        };
        SassieApi sassieApi;

        public HondaCPOInspectionReport()
        {
            sassieApi = new SassieApi();

            presale_list.Add(presale_mappingA);
            presale_list.Add(presale_mappingB);
            presale_list.Add(presale_mappingC);
            presale_list.Add(presale_mappingD);
            presale_list.Add(presale_mappingE);
            presale_list.Add(presale_mappingF);
            presale_list.Add(presale_mappingG);
            presale_list.Add(presale_mappingH);
            presale_list.Add(presale_mappingI);
            presale_list.Add(presale_mappingJ);

            postsale_list.Add(postsale_mappingA);
            postsale_list.Add(postsale_mappingB);
            postsale_list.Add(postsale_mappingC);
            postsale_list.Add(postsale_mappingD);
            postsale_list.Add(postsale_mappingE);
            postsale_list.Add(postsale_mappingF);
            postsale_list.Add(postsale_mappingG);
            postsale_list.Add(postsale_mappingH);
            postsale_list.Add(postsale_mappingI);
            postsale_list.Add(postsale_mappingJ);
        }

        public void GetData()
        {
            try
            {
                //_sAID = "26228303";
                _sAID = "23183043";
                _sCurrentLanguage = "en";

                _dsCPOData = new DBHondaCPO().GetHondaCPOOCR(Convert.ToInt32(_sAID), _sCurrentLanguage);

                int iCtr = 0;


                for (iCtr = 0; iCtr <= _dsCPOData.Tables.Count - 1; iCtr++)
                {
                    Array.Resize(ref _dvCPOData, iCtr + 1);
                    _dvCPOData[iCtr] = new System.Data.DataView(_dsCPOData.Tables[iCtr]);
                }

                //string json2 = JsonConvert.SerializeObject(_dsCPOData.Tables[3]);

                //_strImagePath = _dvCPOData[0].Table.Rows[0]["PDF_File_Name"].ToString().Trim().Remove(0, 11);

                _NoPostSaleVehicles = Convert.ToInt32(_dvCPOData[0].Table.Rows[0]["OVehInspected"]);

                _NoPreSaleVehicles = Convert.ToInt32(_dvCPOData[0].Table.Rows[0]["RVehInspected"]);

                //_DivisionCode = _dvCPOData[0].Table.Rows[0]["Division_Code"].ToString().Trim();
                //if ((_DivisionCode == "B"))
                //{
                //    _ReportTitle = "ReportTitle_Acura";
                //}
                //else
                //{
                //    _ReportTitle = "ReportTitle_Honda";
                //}

                //VEHICLES
               // PopulateVehicles();

                //CONSULTATION INFORMATION  
                // ConsultationInformation();

                //DEALER CONTACT INFORMATION  
                // DealerInformation();

                //FACILITY INSPECTION  
                //FacilityInspection();

                //PRE-SALE
               // PopulatePresaleQuestions();

                //POST-SALE
               // PopulatePostsaleQuestions();

                string json = JsonConvert.SerializeObject(result_dict);

                var data = new
                {
                    grant_type = "client_credentials",
                    client_id = "WSwDiUqqv5Q2InctWBHkWeTWmDmfiNJl",
                    client_secret = "62UEIr61r2FQc9xyvRn4PBdmRQ4gTPwa"
                };
                json = JsonConvert.SerializeObject(data);
                sassieApi.AuthenticateAsync(json);
                //sassieApi.Authenticate(json);


                //Vehicle Compliance Information 	
                if (_dvCPOData[1].Table.Rows.Count > 0)
                {
                    // CreateVehicleComplianceInfo();
                }
                _dsCPOData = null;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void PopulatePresaleQuestions()
        {
            string vin_num;
            Dictionary<int, string> q_mapping;
            int qid;
            int ind = 0;
            foreach (var pair in presale_vehicles)
            {
                vin_num = pair.Key;

                q_mapping = presale_list[ind];

                result_dict.Add(q_mapping[ind], "Yes");

                foreach (var item in vehicle_detail)
                {
                    if (!q_mapping.ContainsKey(item.Key))
                    {
                        continue;
                    }

                    result_dict.Add(q_mapping[item.Key], pair.Value[item.Value].Trim());
                }

                foreach (DataRow row in _dsCPOData.Tables[5].Rows)
                {
                    qid = (int)row["Question_ID"];
                    if (!q_mapping.ContainsKey(qid))
                    {
                        continue;
                    }

                    if (row[vin_num].ToString().ToLower().Equals("no"))
                    {
                        throw new Exception("comments question required!!");
                    }
                    result_dict.Add(q_mapping[qid], row[vin_num].ToString().Trim());
                }
                ind++;
            }

        }

        private void PopulatePostsaleQuestions()
        {
            string vin_num;
            Dictionary<int, string> q_mapping;
            int qid;
            int ind = 0;
            foreach (var pair in postsale_vehicles)
            {
                vin_num = pair.Key;
                q_mapping = postsale_list[ind];

                result_dict.Add(q_mapping[ind], "Yes");

                foreach (var item in vehicle_detail)
                {
                    if (!q_mapping.ContainsKey(item.Key))
                    {
                        continue;
                    }

                    result_dict.Add(q_mapping[item.Key], pair.Value[item.Value].Trim());
                }

                foreach (DataRow row in _dsCPOData.Tables[3].Rows)
                {
                    qid = (int)row["Question_ID"];
                    if (!q_mapping.ContainsKey(qid))
                    {
                        continue;
                    }
                    if (row[vin_num].ToString().ToLower().Equals("no"))
                    {
                        throw new Exception("comments question required!!");
                    }
                    result_dict.Add(q_mapping[qid], row[vin_num].ToString().Trim());
                }
                ind++;
            }

        }

        private void PopulateVehicles()
        {
            string vin;
            Dictionary<string, string> detail;
            foreach (DataRow item in _dsCPOData.Tables[1].Rows)
            {
                vin = item["Vehicle_VIN"].ToString();
                detail = new Dictionary<string, string>()
                    {
                         {"VIN",item["Vehicle_VIN"].ToString() },
                         {"Manufacturer","" },
                        {"Make_Description",item["Make_Description"].ToString() },
                        {"Model_Description", item["Model_Description"].ToString()},
                        {"Vehicle_Year", item["Vehicle_Year"].ToString() },
                        {"Stock_Number", item["Stock_ID"].ToString() },
                        {"Tier", item["Tier_Data"].ToString() },
                    };

                if (item["Audit_Type"].Equals("Pre"))
                {
                    presale_vehicles.Add(vin, detail);
                }
                else
                {
                    postsale_vehicles.Add(vin, detail);
                }
            }
        }

        private void FacilityInspection()
        {
            //question_141
            //_dvCPOData[6].Table.Rows[1]["Question_Value"]
            result_dict.Add("question_141", _dvCPOData[6].Table.Rows[1]["Question_Value"].ToString());
            //question_161
            //_dvCPOData[6].Table.Rows[2]["Question_Value"]
            result_dict.Add("question_161", _dvCPOData[6].Table.Rows[2]["Question_Value"].ToString());
            //question_181
            //_dvCPOData[6].Table.Rows[3]["Question_Value"]
            result_dict.Add("question_181", _dvCPOData[6].Table.Rows[3]["Question_Value"].ToString());
            //question_201
            //_dvCPOData[6].Table.Rows[4]["Question_Value"]
            result_dict.Add("question_201", _dvCPOData[6].Table.Rows[4]["Question_Value"].ToString());
        }

        private void DealerInformation()
        {
            //question_31
            //_dvCPOData[0][0]["Dealer_Contact1"]
            result_dict.Add("question_31", _dvCPOData[0][0]["Dealer_Contact1"].ToString());
            //question_41
            //_dvCPOData[0][0]["Email_Address1"]
            result_dict.Add("question_41", _dvCPOData[0][0]["Email_Address1"].ToString());
            //question_51
            //_dvCPOData[0][0]["Dealer_Contact2"]
            result_dict.Add("question_51", _dvCPOData[0][0]["Dealer_Contact2"].ToString());
            //question_61
            //_dvCPOData[0][0]["Email_Address2"]
            result_dict.Add("question_61", _dvCPOData[0][0]["Email_Address2"].ToString());
            //question_71
            //_dvCPOData[0][0]["Dealer_Contact3"]
            result_dict.Add("question_71", _dvCPOData[0][0]["Dealer_Contact3"].ToString());
            //question_81
            //_dvCPOData[0][0]["Email_Address3"]
            result_dict.Add("question_81", _dvCPOData[0][0]["Email_Address3"].ToString());
            //question_91
            //_dvCPOData[0][0]["Dealer_Contact4"]
            result_dict.Add("question_91", _dvCPOData[0][0]["Dealer_Contact4"].ToString());
            //question_101
            //_dvCPOData[0][0]["Email_Address4"]
            result_dict.Add("question_101", _dvCPOData[0][0]["Email_Address4"].ToString());
        }

        private void ConsultationInformation()
        {
            //question_11
            //_dvCPOData[0][0]["Assignment_ID"]
            result_dict.Add("question_11", (_dvCPOData[0][0]["Assignment_ID"].ToString()));
            //question_2741
            //_dvCPOData[0][0]["Inspector_ID"]
            result_dict.Add("question_2741", _dvCPOData[0][0]["Inspector_ID"].ToString());
            //question_1
            //Convert.ToDateTime(_dvCPOData[0][0]["Audit_Date"]).ToShortDateString()
            result_dict.Add("question_1", Convert.ToDateTime(_dvCPOData[0][0]["Audit_Date"]).ToShortDateString());
            //question_21
            //Convert.ToDateTime(_dvCPOData[0][0]["Audit_Date"]).ToShortTimeString()
            result_dict.Add("question_21", Convert.ToDateTime(_dvCPOData[0][0]["Audit_Date"]).ToShortTimeString());
        }
    }
}
