/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { MessageUserDto } from "./message-user-dto";
import { MessageChannelDto } from "./message-channel-dto";
import { MessageServerDto } from "./message-server-dto";

export class MessageDto {
    content: string;
    user: MessageUserDto;
    channel: MessageChannelDto;
    server: MessageServerDto;
    sentAt: Date;
}
