declare module '@iconscout/react-unicons' {
  import type { SVGProps } from 'react';

  export interface UniconProps extends SVGProps<SVGSVGElement> {
    color?: string;
    size?: string | number;
  }

  export type UniconComponent = (props: UniconProps) => JSX.Element;

  export const UilAngleDown: UniconComponent;
  export const UilBell: UniconComponent;
  export const UilBookOpen: UniconComponent;
  export const UilCalendarAlt: UniconComponent;
  export const UilClipboardNotes: UniconComponent;
  export const UilEditAlt: UniconComponent;
  export const UilEstate: UniconComponent;
  export const UilFileAlt: UniconComponent;
  export const UilPlus: UniconComponent;
  export const UilSearch: UniconComponent;
  export const UilShieldCheck: UniconComponent;
  export const UilTrashAlt: UniconComponent;
  export const UilUserPlus: UniconComponent;
  export const UilUsersAlt: UniconComponent;
}
