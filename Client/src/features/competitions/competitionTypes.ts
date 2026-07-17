import type { PagedResponse } from "../matches/matchTypes";

export type CompetitionType = 1 | 2 | 3;

export type CompetitionResponseDto = {
  id: string;
  name: string;
  country: string;
  logoUrl: string | null;
  type: CompetitionType;
  isActive: boolean;
};

export type CreateCompetitionRequest = {
  name: string;
  country: string;
  type: CompetitionType;
  logoUrl?: string | null;
  isActive?: boolean;
};

export type UpdateCompetitionRequest = {
  id: string;
  name: string;
  country: string;
  type: CompetitionType;
  logoUrl?: string | null;
  isActive: boolean;
};

export type CreatedCompetitionResponseDto = {
  id: string;
  name: string;
};

export const COMPETITION_TYPE = {
  League: 1,
  Cup: 2,
  Hybrid: 3,
} as const;

export const COMPETITION_TYPE_LABEL: Record<CompetitionType, string> = {
  1: "Lig",
  2: "Kupa",
  3: "Hibrit",
};

export const COMPETITION_TYPE_ICON: Record<CompetitionType, string> = {
  1: "sports_soccer",
  2: "emoji_events",
  3: "public",
};

export const COMPETITION_COUNTRY_OPTIONS = [
  "Türkiye",
  "İngiltere",
  "İspanya",
  "Almanya",
  "İtalya",
  "Fransa",
  "Avrupa (Kıtasal)",
  "Dünya",
] as const;

export type CompetitionsPageResult = PagedResponse<CompetitionResponseDto>;
