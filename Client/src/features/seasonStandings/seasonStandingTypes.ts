export type UserPreviewDto = {
  id: string;
  username: string;
  profileImageUrl: string | null;
  roleName: string;
};

export type SeasonStandingPreviewDto = {
  rank: number;
  userId: string;
  user: UserPreviewDto;
  totalPoints: number;
  exactScoreCount: number;
  scoredPredictionCount: number;
};

export function getInitials(username: string): string {
  const parts = username.trim().split(/\s+/).filter(Boolean);

  if (parts.length === 0) {
    return "?";
  }

  if (parts.length === 1) {
    return parts[0].slice(0, 2).toUpperCase();
  }

  return `${parts[0][0] ?? ""}${parts[1][0] ?? ""}`.toUpperCase();
}

export function formatTopPercent(rank: number, totalParticipants: number): string | null {
  if (totalParticipants <= 0 || rank <= 0) {
    return null;
  }

  const percent = Math.max(1, Math.ceil((rank / totalParticipants) * 100));
  return `İlk %${percent}`;
}

export function formatAccuracy(exactScoreCount: number, scoredPredictionCount: number): string | null {
  if (scoredPredictionCount <= 0) {
    return null;
  }

  const percent = Math.round((exactScoreCount / scoredPredictionCount) * 100);
  return `%${percent} isabet`;
}
