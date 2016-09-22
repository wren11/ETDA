
//updated for 7.41
const int senderOffset = 0x0073D958;   // sender class pointer
const int sendOffset = 0x00563E00;     // write to send buffer
const int sendPacketout = 0x00567FB0;  // packet sent

const int MagicSendToken = 0x00569E00;


const int recvPacketin = 0x00467060;   // packet received
const int userNameoffset = 0x0073D910; // 7.41 username
const int Flag = 0x00720000;           // for injecting (empty cave)
const int Options = 0x00750000;        // for Setting Toggles.

const int OptionA = 0x00750002;        // for Setting Toggles.
const int OptionB = 0x00750004;        // for Setting Toggles.
const int OptionC = 0x00750006;        // for Setting Toggles.
const int OptionD = 0x00750008;        // for Setting Toggles.
const int OptionE = 0x00750010;        // for Setting Toggles.
const int OptionF = 0x00750012;        // for Setting Toggles.



const int hPaintPtr = 0x004AC910;      // end Paint 7.41
const int hOnCharacter = 0x004C1B60;   // On Character Login Function 7.41

//GDI
const int DA741_GETDC = 0x004AC8C0;

//ETDA for DA 7.41
const int RecvConsumerPacketAvailable = 0x00721000;
const int RecvConsumerPacketType      = 0x00721004;
const int RecvConsumerPacketLength    = 0x00721008;
const int RecvConsumerPacketData      = 0x00721012;
const int SendConsumerPacketAvailable = 0x006FD000;
const int SendConsumerPacketType      = 0x006FD004;
const int SendConsumerPacketLength    = 0x006FD008;
const int SendConsumerPacketData      = 0x006FD012;
