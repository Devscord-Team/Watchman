/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { ArgumentInfoDto } from "./argument-info-dto";
import { DescriptionDto } from "./description-dto";

export class HelpInformationDto {
    name: string;
    methodFullName: string;
    arguments: ArgumentInfoDto[];
    descriptions: DescriptionDto[];
    isDefault: boolean;
}
