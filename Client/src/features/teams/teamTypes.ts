import type { PagedResponse } from "../matches/matchTypes";

export type TeamResponseDto = {
  id: string;
  name: string;
  shortName: string;
  country: string;
  crestUrl: string | null;
  isActive: boolean;
};

export type TeamPreviewDto = {
  id: string;
  name: string;
  shortName: string;
  crestUrl: string | null;
};

export type CreatedTeamResponseDto = {
  id: string;
  name: string;
};

export type TeamsPageResult = PagedResponse<TeamResponseDto>;

export const TEAM_COUNTRY_OPTIONS = [
  "Türkiye",
  "İngiltere",
  "İspanya",
  "Almanya",
  "İtalya",
  "Fransa",
  "Portekiz",
  "Hollanda",
  "Belçika",
  "Brezilya",
  "Arjantin",
] as const;

export function teamInitials(shortName: string, name: string): string {
  const fromShort = shortName.trim().slice(0, 3).toUpperCase();

  if (fromShort.length >= 2) {
    return fromShort;
  }

  return name
    .split(/\s+/)
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part[0]?.toUpperCase() ?? "")
    .join("");
}
