
//updated for 7.41
const int senderOffset = 0x0073D958;   // sender class pointer
const int sendOffset = 0x00563E00;     // write to send buffer
const int sendPacketout = 0x00567FB0;  // packet sent
const int recvPacketin = 0x00467060;   // packet received
const int userNameoffset = 0x0073D910; // 7.41 username
const int Flag = 0x00720000;           // for injecting (empty cave)
const int Options = 0x00750000;        // for Setting Toggles.