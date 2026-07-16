export type CompetitionSeasonStatus = 1 | 2 | 3 | 4;

export type CompetitionSeasonPreviewDto = {
  id: string;
  competitionId: string;
  name: string;
  status: CompetitionSeasonStatus;
  isActive: boolean;
};

export type CompetitionPreviewDto = {
  id: string;
  name: string;
  logoUrl: string | null;
  type: number;
};

export type CompetitionSeasonResponseDto = {
  id: string;
  competitionId: string;
  competition: CompetitionPreviewDto;
  name: string;
  startDate: string;
  endDate: string;
  status: CompetitionSeasonStatus;
  isActive: boolean;
};

export type MySeasonStandingDto = {
  competitionSeasonId: string;
  rank: number;
  totalPoints: number;
  exactScoreCount: number;
  scoredPredictionCount: number;
  totalParticipants: number;
};

export const COMPETITION_SEASON_STATUS_LABEL: Record<CompetitionSeasonStatus, string> = {
  1: "Yakında",
  2: "Aktif",
  3: "Tamamlandı",
  4: "İptal",
};

const SELECTED_SEASON_STORAGE_KEY = "rightOnScore.selectedSeasonId";

export function readStoredSelectedSeasonId(): string | null {
  try {
    return localStorage.getItem(SELECTED_SEASON_STORAGE_KEY);
  } catch {
    return null;
  }
}

export function writeStoredSelectedSeasonId(seasonId: string | null): void {
  try {
    if (!seasonId) {
      localStorage.removeItem(SELECTED_SEASON_STORAGE_KEY);
      return;
    }

    localStorage.setItem(SELECTED_SEASON_STORAGE_KEY, seasonId);
  } catch {
    // Ignore storage failures (private mode, etc.).
  }
}
